using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Aethel.Campaign;
using Aethel.WorldMemory;

internal sealed class CampaignTestStore : ICampaignStore
{
    internal CampaignState State = CampaignState.Create("campaign", "character", "twilight_bastion");
    internal bool Fail, LoseCas;
    public CampaignState Load() { return State.Copy(); }
    public bool TryCommit(long expected, CampaignState candidate)
    {
        if (Fail) throw new IOException("Injected failure before commit");
        if (LoseCas || State.Version != expected) return false;
        candidate.Validate(); candidate.AssertExtends(State); State = candidate.Copy(); return true;
    }
}
internal sealed class TrainingTestAccess : ITrainingAccess
{
    internal bool Allowed = true;
    public bool CanTrain(string characterId, string classId) { return Allowed; }
}
internal static class CampaignTests
{
    static int checks;
    static string Token() { return Guid.NewGuid().ToString("D"); }
    static void Check(bool value, string label) { checks++; if (!value) throw new Exception(label); }
    static void Status(ProgressionResult result, ProgressionStatus expected)
    { Check(result.Status == expected, "Expected " + expected + ", got " + result.Status); }
    static void MemoryStatus(CommandResult result, CommandStatus expected)
    { Check(result.Status == expected, "Memory expected " + expected + ", got " + result.Status); }
    static bool Valid(CampaignState s) { try { s.Validate(); return true; } catch (SerializationException) { return false; } }
    static void AggregateReward()
    {
        foreach (string choice in new[] { "judge_suspects", "shelter_villagers" })
        {
            var store = new CampaignTestStore(); var memory = new MemoryService(new CampaignMemoryAdapter(store));
            foreach (string clue in MemoryService.ForestClues) memory.ObserveClue(clue, memory.Read().Version);
            long v = memory.Read().Version;
            store.Fail = true;
            MemoryStatus(memory.Decide("act1.spy_hunt", choice, v), CommandStatus.PersistenceFailed);
            Check(!store.State.ForestRewardClaimed && store.State.Gold == 0 && store.State.RitualCharge == 0 && store.State.ReservedHealthPotions == 0, "Partial reward on failed choice");
            store.Fail = false; store.LoseCas = true;
            MemoryStatus(memory.Decide("act1.spy_hunt", choice, v), CommandStatus.Conflict);
            Check(store.State.RitualCharge == 0, "CAS failure changed memory");
            store.LoseCas = false;
            MemoryStatus(memory.Decide("act1.spy_hunt", choice, v), CommandStatus.Accepted);
            Check(store.State.ForestRewardClaimed && store.State.RitualCharge == 1, "Missing aggregate receipt");
            Check(choice == "judge_suspects" ? store.State.Gold == 25 : store.State.Inventory.Total("potion_health_small") == 3, "Wrong canonical reward");
            MemoryStatus(memory.Decide("act1.spy_hunt", choice, v), CommandStatus.Replayed);
            MemoryStatus(memory.Decide("act1.spy_hunt", choice == "judge_suspects" ? "shelter_villagers" : "judge_suspects", memory.Read().Version), CommandStatus.Conflict);
            memory.VisitCitadel(memory.Read().Version); memory.VisitCitadel(memory.Read().Version);
            Check(store.State.RitualCharge == 1 && (choice == "judge_suspects" ? store.State.Gold == 25 : store.State.Inventory.Total("potion_health_small") == 3), "Repeated reward");
            var invalid = store.State.Copy(); invalid.ForestRewardClaimed = false; Check(!Valid(invalid), "Missing reward receipt accepted");
        }
        var overflow = new CampaignTestStore(); overflow.State.Gold = int.MaxValue;
        var m = new MemoryService(new CampaignMemoryAdapter(overflow));
        foreach (string c in MemoryService.ForestClues) m.ObserveClue(c, m.Read().Version);
        MemoryStatus(m.Decide("act1.spy_hunt", "judge_suspects", m.Read().Version), CommandStatus.PersistenceFailed);
        Check(overflow.State.RitualCharge == 0 && overflow.State.Gold == int.MaxValue, "Gold overflow corrupted transaction");
    }
    static void Progression()
    {
        var store = new CampaignTestStore(); var access = new TrainingTestAccess(); var service = new ProgressionService(store, access);
        string defense = "twilight_bastion.defense", rupture = "twilight_bastion.rupture";
        Status(service.SelectPath(Token(), defense, 0), ProgressionStatus.Locked);
        Status(service.SetConfirmedLevel(Token(), 4, service.Read().Version), ProgressionStatus.Accepted);
        Status(service.DismissNotice(Token(), service.Read().Version), ProgressionStatus.Locked);
        Status(service.SetConfirmedLevel(Token(), 5, service.Read().Version), ProgressionStatus.Accepted);
        string levelToken = Token(); Status(service.SetConfirmedLevel(levelToken, 10, service.Read().Version), ProgressionStatus.Accepted);
        Status(service.SetConfirmedLevel(levelToken, 10, 0), ProgressionStatus.Replayed);
        Status(service.SetConfirmedLevel(levelToken, 15, service.Read().Version), ProgressionStatus.Conflict);
        Check(service.Read().EarnedSpecializationPoints == 2 && service.Read().Nodes.Count == 0, "Delayed milestone points lost");
        access.Allowed = false;
        Status(service.SelectPath(Token(), defense, service.Read().Version), ProgressionStatus.Locked);
        access.Allowed = true;
        string dismiss = Token(); Status(service.DismissNotice(dismiss, service.Read().Version), ProgressionStatus.Accepted);
        string select = Token(); long expected = service.Read().Version;
        store.Fail = true; Status(service.SelectPath(select, defense, expected), ProgressionStatus.PersistenceFailed);
        Check(service.Read().Nodes.Count == 0 && service.Read().PrimaryPath == "", "Failed selection leaked");
        store.Fail = false;
        Status(service.SelectPath(select, defense, expected), ProgressionStatus.Accepted);
        Status(service.SelectPath(select.ToUpperInvariant(), defense, expected), ProgressionStatus.Replayed);
        Status(service.SelectPath(select, rupture, service.Read().Version), ProgressionStatus.Conflict);
        Check(service.Read().Nodes.Count == 1 && service.Read().UnspentSpecializationPoints == 1 && service.Read().NoticeDismissed, "Selection erased notice or points");
        Status(service.BuyNode(Token(), defense + ".t3", service.Read().Version), ProgressionStatus.Locked);
        Status(service.BuyNode(Token(), "rune_weaver.control.t1", service.Read().Version), ProgressionStatus.Invalid);
        Status(service.BuyNode(Token(), rupture + ".t1", service.Read().Version), ProgressionStatus.Accepted);
        Status(service.BuyNode(Token(), defense + ".t2", service.Read().Version), ProgressionStatus.Locked);
        Status(service.SetConfirmedLevel(Token(), 30, service.Read().Version), ProgressionStatus.Accepted);
        Status(service.BuyNode(Token(), defense + ".t2", service.Read().Version), ProgressionStatus.Accepted);
        Status(service.BuyNode(Token(), defense + ".t3", service.Read().Version), ProgressionStatus.Accepted);
        Status(service.BuyNode(Token(), defense + ".t4", service.Read().Version), ProgressionStatus.Accepted);
        Status(service.BuyNode(Token(), rupture + ".t2", service.Read().Version), ProgressionStatus.Accepted);
        Status(service.BuyNode(Token(), rupture + ".t3", service.Read().Version), ProgressionStatus.Locked);
        Check(service.Read().Nodes.Count == 6 && service.Read().UnspentSpecializationPoints == 0, "Six-point budget");
        Status(service.SetConfirmedLevel(Token(), 4, service.Read().Version), ProgressionStatus.Conflict);
        Status(service.SetConfirmedLevel(Token(), 31, service.Read().Version), ProgressionStatus.Invalid);
        Status(service.SelectPath("bad-token", defense, service.Read().Version), ProgressionStatus.Invalid);
        Status(service.DismissNotice(Token(), service.Read().Version - 1), ProgressionStatus.Conflict);
        var detached = service.Read(); detached.Nodes.Clear(); Check(service.Read().Nodes.Count == 6, "Snapshot alias escaped");
    }
    static void RespecTransactions()
    {
        var store = new CampaignTestStore(); var access = new TrainingTestAccess(); var service = new ProgressionService(store, access);
        string a = "twilight_bastion.defense", b = "twilight_bastion.rupture";
        service.SetConfirmedLevel(Token(), 5, service.Read().Version); service.SelectPath(Token(), a, service.Read().Version);
        string free = Token(); long v = service.Read().Version;
        store.Fail = true;
        Status(service.Respec(free, b, new[] { b + ".t1" }, v), ProgressionStatus.PersistenceFailed);
        Check(service.Read().RespecCount == 0 && service.Read().PrimaryPath == a, "Free respec consumed on failure");
        store.Fail = false;
        Status(service.Respec(free, b, new[] { b + ".t1" }, v), ProgressionStatus.Accepted);
        Status(service.Respec(free, b, new[] { b + ".t1" }, v), ProgressionStatus.Replayed);
        Check(service.RespecPrice() == 25 && service.Read().RespecCount == 1, "Respec price");
        Status(service.Respec(Token(), a, new[] { a + ".t1" }, service.Read().Version), ProgressionStatus.InsufficientGold);
        store.State.Gold = 100; // fixture, not an application gold-award API
        string paid = Token(); v = service.Read().Version;
        access.Allowed = false; Status(service.Respec(paid, a, new[] { a + ".t1" }, v), ProgressionStatus.Locked); access.Allowed = true;
        store.Fail = true; Status(service.Respec(paid, a, new[] { a + ".t1" }, v), ProgressionStatus.PersistenceFailed);
        Check(service.Read().Gold == 100 && service.Read().RespecCount == 1, "Failed paid respec charged");
        store.Fail = false;
        Status(service.Respec(paid, a, new[] { a + ".t1" }, v), ProgressionStatus.Accepted);
        Status(service.Respec(paid, a, new[] { a + ".t1" }, v), ProgressionStatus.Replayed);
        Check(service.Read().Gold == 75 && service.Read().RespecCount == 2, "Respec replay recharged");
        Status(service.Respec(Token(), a, new[] { a + ".t1" }, service.Read().Version), ProgressionStatus.Replayed);
        Check(service.Read().Gold == 75, "No-op respec charged");
        Status(service.Respec(Token(), b, new[] { b + ".t1", b + ".t1" }, service.Read().Version), ProgressionStatus.Invalid);
        Status(service.Respec(Token(), b, new[] { b + ".t2" }, service.Read().Version), ProgressionStatus.Invalid);
        Status(service.Respec(Token(), b, null, service.Read().Version), ProgressionStatus.Invalid);
        Check(service.Read().Memory.Choices.Count == 0, "Respec changed narrative");
        var history = new CampaignTestStore();
        history.State.Memory.Version = 512;
        for (int i = 0; i < 512; i++) { string id = Token(); history.State.Commands.Add(id, "test"); history.State.ReceiptVersions.Add(id,i); }
        string evicted = history.State.ReceiptVersions.Single(x=>x.Value==0).Key;
        var capped = new ProgressionService(history, access);
        Status(capped.SetConfirmedLevel(Token(), 5, 512), ProgressionStatus.Accepted);
        Status(capped.SetConfirmedLevel(evicted, 5, 0), ProgressionStatus.Expired);
        Check(capped.Read().Level == 5 && capped.Read().Commands.Count == 512 && capped.Read().ReceiptFloor == 1, "Journal compaction");
    }
    static void AllocationMatrix()
    {
        int[] tierLevel = { 0, 5, 10, 20, 30 };
        foreach (string cls in SpecializationCatalog.Classes)
        for (int level = 1; level <= 30; level++)
        for (int primary = 0; primary < 2; primary++)
        for (int left = 0; left <= 4; left++)
        for (int right = 0; right <= 4; right++)
        {
            var paths = SpecializationCatalog.Paths(cls);
            var s = CampaignState.Create("matrix", "character", cls); s.Level = level; s.TotalXp = ExperienceRules.MinimumForLevel(level);
            s.PrimaryPath = paths[primary];
            for (int i = 1; i <= left; i++) s.Nodes.Add(paths[0] + ".t" + i);
            for (int i = 1; i <= right; i++) s.Nodes.Add(paths[1] + ".t" + i);
            bool expected = level >= 5 && (primary == 0 ? left > 0 : right > 0) &&
                left + right <= (level < 5 ? 0 : 1 + (level - 5) / 5) && left + right <= 6 &&
                tierLevel[left] <= level && tierLevel[right] <= level &&
                ((primary == 0 ? right == 0 : left == 0) || level >= 10);
            Check(Valid(s) == expected, "Allocation mismatch " + cls + ": " + level + ", " + left + "+" + right);
        }
    }
    static void ThreatMatrix()
    {
        for (int player = 1; player <= 30; player++)
        for (int enemy = 1; enemy <= 30; enemy++)
        foreach (EncounterCategory category in Enum.GetValues(typeof(EncounterCategory)))
        {
            int d = enemy - player;
            var v = ThreatPresentation.Compare(player, enemy, category);
            Check(v.Band == (d >= 4 ? ThreatBand.FarAbove : d > 0 ? ThreatBand.Above : d == 0 ? ThreatBand.Equal : ThreatBand.Below), "Threat band");
            Check(v.Category == category && v.EnemyLevel == enemy && v.ShowThreatSymbol == (d >= 4), "Threat mutated category");
            Check(v.ShowBossBar == (category == EncounterCategory.Boss || category == EncounterCategory.SubBoss), "Boss bar lost below level");
            Check(!string.IsNullOrEmpty(v.AccessibleLabel) && !string.IsNullOrEmpty(v.ColorRole), "Missing accessible indication");
        }
        foreach (var pair in new[] { new[] { 0, 1 }, new[] { 1, 31 }, new[] { -1, 10 } })
        {
            bool bad = false; try { ThreatPresentation.Compare(pair[0], pair[1], EncounterCategory.Common); } catch (ArgumentOutOfRangeException) { bad = true; }
            Check(bad, "Invalid level accepted");
        }
    }
    static void FullCampaignComposition()
    {
        var store = new CampaignTestStore(); var access = new TrainingTestAccess();
        var memory = new MemoryService(new CampaignMemoryAdapter(store));
        var progression = new ProgressionService(store, access);
        foreach (string c in MemoryService.ForestClues) memory.ObserveClue(c, memory.Read().Version);
        for (int i = 0; i < CanonicalCampaign.Quests.Length; i++)
        {
            var quest = CanonicalCampaign.Quests[i];
            memory.EnterZone(quest.Zone, memory.Read().Version);
            memory.ConfirmEncounter(quest.Zone, memory.Read().Version);
            MemoryStatus(memory.Decide(quest.Id, quest.Choices[0].Id, memory.Read().Version), CommandStatus.Accepted);
            if (i == 0)
            {
                progression.SetConfirmedLevel(Token(), 5, progression.Read().Version);
                progression.SelectPath(Token(), "twilight_bastion.defense", progression.Read().Version);
            }
            Check(store.State.RitualCharge == i + 1 && store.State.Gold == 25, "Composition lost progress or repeated reward");
        }
        memory.RevealEpilogue(memory.Read().Version);
        Check(memory.Read().IsEpilogue && progression.Read().Nodes.Count == 1 && memory.Read().Facts.Length == 24, "Full aggregate epilogue");
        foreach (var q in CanonicalCampaign.Quests)
        {
            var r = CampaignContent.FindRegion(q.Zone);
            Check(r != null && r.ZoneId == q.Zone && r.MainEncounterLevel <= 30, "Region content absent");
            Check(q.Order == 12 ? !r.HasCommonEnemies : r.CommonMin >= 1 && r.CommonMax >= r.CommonMin, "Invalid common range");
        }
        foreach (string hub in new[] { "citadel", "dunes_camp", "underground_refuge", "liminal_post", "unknown" })
            Check(CampaignContent.FindRegion(hub) == null, "Hostile hub recommendation");
        var stale = progression.Read();
        store.LoseCas = true;
        Status(progression.SetConfirmedLevel(Token(), 10, stale.Version), ProgressionStatus.Conflict);
        Check(progression.Read().Level == 5 && progression.Read().RitualCharge == 12, "Lost CAS changed level/narrative");
    }
    static void DiskIntegration()
    {
        string dir = Path.Combine(Path.GetTempPath(), "aethel-campaign-" + Token());
        string path = Path.Combine(dir, "campaign.json");
        var initial = CampaignState.Create("disk", "character", "twilight_bastion");
        try
        {
            var store = new JsonCampaignStore(path, initial); var access = new TrainingTestAccess();
            var memory = new MemoryService(new CampaignMemoryAdapter(store));
            foreach (string c in MemoryService.ForestClues) memory.ObserveClue(c, memory.Read().Version);
            memory.Decide("act1.spy_hunt", "judge_suspects", memory.Read().Version);
            var service = new ProgressionService(store, access);
            service.SetConfirmedLevel(Token(), 5, service.Read().Version);
            string pathId = "twilight_bastion.defense", select = Token(); long v = service.Read().Version;
            service.SelectPath(select, pathId, v);
            var reloaded = new ProgressionService(new JsonCampaignStore(path, initial), access);
            Check(reloaded.Read().Gold == 25 && reloaded.Read().RitualCharge == 1 && reloaded.Read().PrimaryPath == pathId, "Aggregate disk reload");
            Status(reloaded.SelectPath(select, pathId, v), ProgressionStatus.Replayed);
            var stale = store.Load(); long oldVersion = stale.Version;
            memory.VisitCitadel(memory.Read().Version);
            stale.Memory.Version++;
            Check(!store.TryCommit(oldVersion, stale), "Stale writer overwrote narrative");
            File.WriteAllText(path + ".abandoned.tmp", "incomplete write");
            Check(store.Load().PrimaryPath == pathId, "Abandoned temp file became authoritative");
            var before = store.Load();
            Status(reloaded.Respec(Token(), "twilight_bastion.rupture", new[] { "twilight_bastion.rupture.t1" }, before.Version), ProgressionStatus.Accepted);
            Check(store.Load().Memory.Revealed.Contains("act1.spy_hunt") && store.Load().Gold == 25, "Respec lost narrative/gold");
            Check(File.Exists(path + ".bak"), "Backup missing");
            var erase = store.Load(); long eraseVersion = erase.Version;
            erase.Memory.Revealed.Clear(); erase.Memory.Version++;
            bool rejectedErase = false;
            try { store.TryCommit(eraseVersion, erase); } catch (SerializationException) { rejectedErase = true; }
            Check(rejectedErase && store.Load().Memory.Revealed.Contains("act1.spy_hunt"), "Confirmed history erased");
            bool wrong = false; try { new JsonCampaignStore(path, CampaignState.Create("disk", "other", "twilight_bastion")).Load(); } catch (SerializationException) { wrong = true; }
            Check(wrong, "Cross-character load accepted");
            File.WriteAllText(path, "{}"); bool corrupt = false;
            try { store.Load(); } catch (SerializationException) { corrupt = true; }
            Check(corrupt, "Corrupt save silently reset");
        }
        finally { if (Directory.Exists(dir)) Directory.Delete(dir, true); }
    }
    public static int Main()
    {
        try { RuntimeTests.Run(); ExtendedRuntimeTests.Run(); AggregateReward(); Progression(); RespecTransactions(); AllocationMatrix(); ThreatMatrix(); FullCampaignComposition(); DiskIntegration();
            Console.WriteLine(checks + " campaign checks passed: transactions, 7500 allocations, 3600 threat comparisons"); return 0; }
        catch (Exception e) { Console.Error.WriteLine(e); return 1; }
    }
}
