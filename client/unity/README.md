# Cliente Unity 6.6 — inspeção U0 e percurso jogável técnico U1

Abra **esta pasta** (`client/unity/`) como projeto no Unity Hub com **Unity 6.6 `6000.6.6f1`**. O projeto `client/unity/Echoes-of-Aethel/` criado localmente pelo Hub é uma cópia separada, ignorada pelo Git. `Assets/Scenes/U0_Inspection.unity` continua como diagnóstico de hardware.

`Assets/Scenes/U1_PlayableRoute.unity` é um percurso técnico curto: Bastião com geometria provisória sai da Cidadela, atravessa o portão, enfrenta um Lobo Etéreo no Bosque e pode retornar. Os limites e pilares têm colisão; o lobo persegue, anuncia o golpe e mantém distância mínima do jogador. O HUD provisório mostra vida, éter, fôlego e objetivo. **Ainda não é o mapa ou visual final:** não há animação, áudio, NPC, quest nem outro inimigo nesta cena; itens recebidos ainda não têm interface de inventário.

Após derrotar o lobo, volte pelo portão até a Cidadela; o objetivo passa a **Percurso U1 concluído**. O proprietário confirmou em 16/10/2026 que matou o lobo e viu essa mensagem ao retornar. Isso valida o ciclo básico no PC, sem medir FPS ou colisão em todos os pontos.

Controles U1: WASD/setas para mover, `1` ou clique esquerdo para básico, `2`/`Q` ou clique direito para Impacto, `3`/`E` para Guarda, `4`/`R` para Fenda, Espaço para dash, `F6` para reiniciar a tentativa e Esc para pausar. Os valores e tempos U1 são experimentais; o dano usa `CombatMath` do núcleo compartilhado.

A vitória do lobo agora usa `EncounterRewardService`: XP, ouro e eventual loot são gravados **uma vez** por ID de encontro no JSON `aethel-u1-campaign.json` de `Application.persistentDataPath`. O HUD mostra o ganho confirmado ou informa que a recompensa já foi recebida. `F6` reinicia a luta, sem duplicar prêmio. Se a gravação falhar, a mensagem avisa e uma nova tentativa pode ser feita; o save próprio U1 não importa nem altera saves Godot/Unreal. HP/MP do combate ainda são locais da cena e não são gravados. Não apague o save para repetir recompensas; a intenção é testar idempotência.

O núcleo de domínio `shared/csharp/` entra como pacote Unity local `com.sarhuman.aethel.domain`, com uma única assembly sem referência à engine. O teste EditMode `U1DomainSmoke.Verify` confirmou os atributos iniciais 172/52, roundtrip JSON, `File.Replace`, `FileStream.Flush(true)` e recompensa única após recarregar. A prova de inicialização do player confirmou que a cena e os atores carregam fora do editor em `-batchmode -nographics`; o novo save/recompensa ainda precisa de teste visual no PC.

O projeto e os manifests foram criados pelo editor instalado em 16/10/2026. Ele usa o renderer padrão da criação mínima; **URP ainda não está instalado**. O build de inspeção pode ser reproduzido no Windows, com o módulo Windows Build Support, executando:

```powershell
& 'C:\Program Files\Unity\Hub\Editor\6000.6.6f1\Editor\Unity.exe' -batchmode -nographics -quit -projectPath '<caminho-absoluto>\client\unity' -executeMethod U0Inspection.CreateSceneAndBuild -logFile '<caminho-absoluto>\unity-u0-build.log'
```

O executável sai em `client/unity/Build/EchoesOfAethel-U0.exe`, diretório ignorado pelo Git. Em 16/10/2026, a compilação terminou com `U0 Windows inspection build succeeded` e o executável iniciou fora do editor em modo `-batchmode -nographics`. O log registrou Unity `6000.6.6f1`, CPU Xeon E5-2630 v3, 16.224 MB de RAM e resolução 1424×720. A GPU apareceu como `Null Device` porque o teste desabilitou a renderização; isso **não valida** a RX 580, qualidade visual, FPS, tempo de frame nem memória de vídeo real. A abertura visual e a medição no PC continuam pendentes.

Para regenerar o percurso U1 e o executável Windows, feche o editor Unity e execute:

```powershell
& 'C:\Program Files\Unity\Hub\Editor\6000.6.6f1\Editor\Unity.exe' -batchmode -nographics -quit -projectPath '<caminho-absoluto>\client\unity' -executeMethod U1PlayableRouteBuilder.CreateSceneAndBuild -logFile '<caminho-absoluto>\unity-u1-build.log'
```

O executável é `client/unity/Build/EchoesOfAethel-U1.exe`. A cena versionada também pode ser aberta e executada no editor. A entrega U1 foi autorizada como protótipo isolado enquanto o gate C0 de conteúdo continua aberto; ela não aprova os 12 mapas nem altera os clientes Godot e Unreal.

[Checagem de portabilidade do núcleo C#](../../docs/engineering/unity-domain-portability.md): importação, smoke EditMode e abertura Windows passaram; a recompensa no player visual e IL2CPP ainda precisam de testes.
