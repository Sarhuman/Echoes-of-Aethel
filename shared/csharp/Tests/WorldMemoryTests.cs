using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Aethel.WorldMemory;

internal sealed class FakeStore : IMemoryStore
{
    internal MemorySave State = new MemorySave { CampaignId = "test-campaign" };
    internal bool Fail;
    public MemorySave Load() { return State.Copy(); }
    public bool TryCommit(long expected, MemorySave candidate)
    {
        if (Fail) throw new IOException("Injected disk failure");
        if (State.Version != expected) return false;
        State = candidate.Copy(); return true;
    }
}
internal static class WorldMemoryTests
{
    static int checks;
    static void Check(bool ok, string label) { checks++; if (!ok) throw new Exception(label); }
    static void Status(CommandResult r, CommandStatus s) { Check(r.Status == s, "Expected " + s + " got " + r.Status); }
    static void Invalid(Action action)
    {
        bool threw = false; try { action(); } catch (SerializationException) { threw = true; }
        Check(threw, "Invalid save was accepted");
    }
    static MemoryService Ready(FakeStore store)
    {
        var service = new MemoryService(store);
        foreach (var c in MemoryService.ForestClues) Status(service.ObserveClue(c, service.Read().Version), CommandStatus.Accepted);
        return service;
    }
    static void Complete(MemoryService service, int order, int option)
    {
        var q = CanonicalCampaign.Quests[order - 1];
        Status(service.EnterZone(q.Zone, service.Read().Version), CommandStatus.Accepted);
        if (order % 3 == 0) Status(service.ConfirmEncounter(q.Zone, service.Read().Version), CommandStatus.Accepted);
        Status(service.Decide(q.Id, q.Choices[option].Id, service.Read().Version), CommandStatus.Accepted);
        if (order % 3 != 0) Status(service.ConfirmEncounter(q.Zone, service.Read().Version), CommandStatus.Accepted);
    }
    static void Transactions()
    {
        var store = new FakeStore(); var service = new MemoryService(store);
        Status(service.Decide("act1.spy_hunt", "judge_suspects", 0), CommandStatus.Locked);
        Status(service.Decide("unknown", "unknown", 0), CommandStatus.Invalid);
        Status(service.EnterZone("crypt_06", 0), CommandStatus.Locked);
        Status(service.RevealEpilogue(0), CommandStatus.Locked);
        service = Ready(store); long v = service.Read().Version;
        Status(service.Decide("act1.spy_hunt", "judge_suspects", v - 1), CommandStatus.Conflict);
        Check(service.Read().Facts.Length == 0, "Conflict leaked facts");
        store.Fail = true;
        Status(service.Decide("act1.spy_hunt", "judge_suspects", v), CommandStatus.PersistenceFailed);
        Check(service.Read().Version == v && service.Read().Facts.Length == 0, "Failed commit mutated state");
        store.Fail = false;
        Status(service.Decide("act1.spy_hunt", "judge_suspects", v), CommandStatus.Accepted);
        Check(service.Read().Facts.Length == 1 && !service.Read().Facts[0].IsDelayed, "Spoiler before return");
        Status(service.Decide("act1.spy_hunt", "judge_suspects", v), CommandStatus.Replayed);
        Status(service.Decide("act1.spy_hunt", "shelter_villagers", service.Read().Version), CommandStatus.Conflict);
        store.Fail = true;
        Status(service.VisitCitadel(service.Read().Version), CommandStatus.PersistenceFailed);
        Check(service.Read().Facts.Length == 1, "Uncommitted consequence leaked");
        store.Fail = false;
        Status(service.VisitCitadel(service.Read().Version), CommandStatus.Accepted);
        v = service.Read().Version;
        Status(service.VisitCitadel(v), CommandStatus.Replayed);
        Check(service.Read().Version == v && service.Read().Facts.Length == 2, "Repeated consequence");
        var detached = store.Load(); detached.Choices.Clear();
        Check(service.Read().Facts.Length == 2, "Snapshot alias escaped");
    }
    static void Milestones()
    {
        var store = new FakeStore(); var service = Ready(store);
        Complete(service, 1, 0); Complete(service, 2, 1);
        service.VisitCitadel(service.Read().Version);
        Check(!service.Read().Facts.Any(f => f.QuestId == "act1.desperate_alchemist" && f.IsDelayed), "Pantom effect early");
        Status(service.Decide("act1.guardian_roots", "seal_sap", service.Read().Version), CommandStatus.Locked);
        Complete(service, 3, 0); service.VisitCitadel(service.Read().Version);
        Check(service.Read().Stage == 1 && service.Read().Facts.Count(f => f.IsDelayed) == 3, "Act I memory");
        Complete(service, 4, 0); Complete(service, 5, 0); service.VisitCitadel(service.Read().Version);
        Check(!service.Read().Facts.Any(f => f.QuestId == "act2.flame_ritual" && f.IsDelayed), "Crypt effect before entry");
        service.EnterZone("crypt_06", service.Read().Version); service.VisitCitadel(service.Read().Version);
        Check(service.Read().Facts.Any(f => f.QuestId == "act2.flame_ritual" && f.IsDelayed), "Crypt effect absent");
        var broken = store.Load(); broken.Schema = 9; Invalid(broken.Validate);
        broken = store.Load(); broken.Choices["act2.ossuary"] = "free_captives"; Invalid(broken.Validate);
        broken = store.Load(); broken.Revealed.Add("act2.lost_caravan"); Invalid(broken.Validate);
        broken = store.Load(); broken.Clues.Add("invented"); Invalid(broken.Validate);
        broken = store.Load(); broken.Entered.Add("heart_12"); Invalid(broken.Validate);
        broken = store.Load(); broken.Encounters.Add("unknown"); Invalid(broken.Validate);
    }
    static void AllRoutes()
    {
        for (int route = 0; route < 4096; route++)
        {
            var store = new FakeStore(); var service = Ready(store);
            for (int order = 1; order <= 12; order++)
            {
                Complete(service, order, (route >> (order - 1)) & 1);
                if (order == 11) Status(service.Decide("act4.heart_fate", "restore_order", service.Read().Version), CommandStatus.Locked);
                service = new MemoryService(store); // reload without visiting hub between maps
            }
            Status(service.RevealEpilogue(service.Read().Version), CommandStatus.Accepted);
            var view = service.Read();
            Check(view.Facts.Length == 24 && view.Stage == 4 && view.IsEpilogue && view.Mercy + view.Judgment == 12, "Route missing memory");
            Check(view.Predominance == (view.Mercy == view.Judgment ? "mixed" : view.Mercy > view.Judgment ? "mercy" : "judgment"), "Affinity tie");
            long version = view.Version; Status(service.VisitCitadel(version), CommandStatus.Replayed);
            Check(service.Read().Version == version, "Return changed completed route");
        }
    }
    static void Disk()
    {
        string dir = Path.Combine(Path.GetTempPath(), "aethel-memory-" + Guid.NewGuid().ToString("N"));
        string path = Path.Combine(dir, "memory.json");
        try
        {
            var disk = new JsonMemoryStore(path, "disk-test"); var service = new MemoryService(disk);
            foreach (var c in MemoryService.ForestClues) service.ObserveClue(c, service.Read().Version);
            service.Decide("act1.spy_hunt", "shelter_villagers", service.Read().Version);
            var stale = disk.Load();
            service.VisitCitadel(service.Read().Version);
            stale.Version++; Check(!disk.TryCommit(stale.Version - 1, stale), "CAS overwrite");
            var reloaded = new MemoryService(new JsonMemoryStore(path, "disk-test"));
            Check(reloaded.Read().Facts.Length == 2 && File.Exists(path + ".bak"), "Disk reload/backup");
            Invalid(() => new JsonMemoryStore(path, "other-campaign").Load());
            var before = File.ReadAllText(path);
            var lockedCandidate = disk.Load(); long lockedVersion = lockedCandidate.Version; lockedCandidate.Version++;
            using (var guard = new FileStream(path + ".lock", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                bool refused = false; try { disk.TryCommit(lockedVersion, lockedCandidate); } catch (IOException) { refused = true; }
                Check(refused, "Exclusive lock ignored");
            }
            Check(File.ReadAllText(path) == before, "Lock failure changed file");
            File.WriteAllText(path, "{}"); Invalid(() => disk.Load());
            File.WriteAllText(path, new string('x', 32769)); Invalid(() => disk.Load());
        }
        finally { if (Directory.Exists(dir)) Directory.Delete(dir, true); }
    }
    public static int Main()
    {
        try { Transactions(); Milestones(); Disk(); AllRoutes(); Console.WriteLine(checks + " checks passed; 4096 C# campaign routes"); return 0; }
        catch (Exception e) { Console.Error.WriteLine(e); return 1; }
    }
}
