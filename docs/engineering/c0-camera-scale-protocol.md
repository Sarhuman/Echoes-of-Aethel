# C0-07 / G00 — protocolo da arena descartável de escala e câmera

**Estado em 15/10/2026: cena técnica versionada via LFS, sem medição visual concluída.** O executável local `UnrealEditor.exe` foi encontrado com versão `++UE5+Release-5.8-CL-58210709`. O build Editor e o teste de atributos já constam do [plano](unreal-execution-plan.md); eles não validam escala, câmera ou desempenho. A consulta automática de CPU/GPU/RAM via CIM retornou `Acesso negado`, portanto o PC-alvo ainda precisa ser registrado por método permitido.

Este spike atende [D02](diablo-direction-decisions.md) e à dependência R1 do plano. É uma **arena técnica descartável**, sem quest, save, loot, combate, IA, progressão ou entrada no menu. Nada dela vira mapa de campanha sem nova revisão. O [atlas](../design/campaign-world-atlas-3d.md) e a [matriz C0-01](c0-map-readiness.md) usarão apenas resultados medidos e aceitos, não as dimensões iniciais abaixo.

## Montagem e variantes

1. Criar um nível temporário sob `/Game/Aethel/Dev/Measurement/` com piso, duas paredes paralelas ajustáveis, coluna opaca, degrau, borda marcada e cinco volumes substitutos de herói. Registrar versão exata do asset e screenshot da planta. Não incluir inimigos nem ligar input de gameplay.
2. Usar um único enquadramento para comparar **perspectiva estreita com FOV inicial experimental de 35°** e **ortográfica**. Manter a mesma resolução, câmera, posição dos volumes, luz e pós-processo; registrar altura, inclinação, distância, FOV ou largura ortográfica e escala aparente de perto/longe.
3. Posicionar os cinco substitutos com alturas/raios **provisórios**, identificados por classe, e um volume de referência de monstro grande e um de invocação. Esses números são parâmetros de teste, não tamanho canônico de herói ou collider final. Testar passagem solo, cruzamento de dois corpos, recuo e travessia junto à coluna/poço.
4. Capturar a cena normal e com névoa/VFX reduzidos. Colocar sinais visuais de ataque no chão de tamanhos diferentes e conferir se coluna, parede, partículas e UI cobrem seu início, extensão ou borda. Esses sinais são apenas formas de teste, sem dano.
5. Repetir na resolução-alvo e em uma resolução menor; anotar versão da engine, hardware, renderer, preset, FPS médio e mínimo observado, enquadramento e custo de GPU/CPU. Um valor de FPS sem resolução e hardware não é evidência.

## Folha de resultados — preencher após execução no editor

| Medida | Perspectiva estreita | Ortográfica | Critério de decisão |
|---|---|---|---|
| Arquivo de nível + revisão | Pendente | Pendente | Mesmo layout e objetos em ambas |
| Câmera (posição, inclinação, FOV/largura) | Pendente; início 35° **EXPERIMENTAL** | Pendente | Hero e ameaça legíveis perto/longe |
| Resolução / escala de UI | Pendente | Pendente | Texto e telegraph continuam legíveis |
| Cápsulas dos cinco heróis e invocação | Pendente | Pendente | Nenhum corpo atravessa obstáculo ou prende retorno |
| Largura mínima de passagem solo | Pendente | Pendente | Travessia e reversão repetíveis |
| Largura mínima para cruzamento/combate | Pendente | Pendente | Sem bloqueio permanente por aliado/inimigo |
| Oclusão por coluna, parede e névoa | Pendente | Pendente | Ameaça/saída/pista não desaparecem |
| Leitura de telegraph com efeitos reduzidos | Pendente | Pendente | Forma e tempo permanecem discerníveis |
| Hardware, renderer, preset, FPS e frametime | Pendente | Pendente | Comparação reproduzível no mesmo PC |
| Capturas e notas de observação | Pendente | Pendente | Evidência visual junto dos números |

**Aceite de C0-07:** registrar ambas as variantes no editor, resolver oclusões e passagem com ao menos duas tentativas de ajuste, escolher uma câmera provisória para as plantas, e publicar medidas com unidade, hardware e capturas. Não usar a meta de “30 monstros a 60 FPS” como resultado deste spike sem IA/combate reais; ela pertence à medição posterior à prova jogável. Se as duas câmeras falharem, D02 volta à revisão.

## Estado da primeira passagem

- Instalação Unreal: **confirmada**, executável 5.8 encontrado. Build do módulo e Automation de atributos: registrados anteriormente no [plano](unreal-execution-plan.md).
- Nível técnico: [`L_C0_ScaleProbe.umap`](../../client/unreal/Content/Aethel/Dev/Measurement/L_C0_ScaleProbe.umap) **versionado por Git LFS**, 40.408 bytes, SHA-256 `549b0f930aed2ea9d80a87358f431658fbe1e3841a81345d252fe3ff7d4a4fec`. O gerador criou três passagens experimentais de 200/300/400 cm, cinco volumes de herói, uma coluna, duas câmeras e duas luzes. São cubos de referência, não cápsulas, personagens ou mapa de campanha.
- [Gerador reproduzível](../../tools/unreal/c0_generate_scale_probe.py): executado no Editor 5.8 com `-ExecutePythonScript=<caminho absoluto com barras /> -nullrhi -DDC-ForceMemoryCache`. Os plugins editoriais `PythonScriptPlugin` e `EditorScriptingUtilities` constam do `.uproject`. A [documentação da Epic](https://dev.epicgames.com/documentation/en-us/unreal-engine/scripting-the-unreal-editor-using-python) descreve Python como ferramenta somente do editor.
- Inspeção estrutural: o nível recarregou com **20 atores C0**. Uma solicitação de captura em 1280 × 720 pela câmera de perspectiva gerou PNG, mas ele saiu **inteiramente preto**, inclusive depois de adicionar luzes. A captura não demonstra aparência, FOV nem iluminação do mapa; pode ser limitação do fluxo automatizado durante a inicialização.
- A abertura gráfica com `UnrealEditor.exe` não apresentou janela e deixou de avançar no log; somente essa instância foi encerrada após a espera. Não houve inspeção de viewport. Cápsulas reais, NavMesh, comparação perspectiva × ortográfica, oclusão, FPS e frametime **continuam sem medição**.
- A tentativa anterior de criar checkout separado causou um erro de aplicação `git.exe`. O envio foi recuperado sem checkout: objeto enviado por `git lfs push --object-id`, ponteiro publicado no PR e hash/tamanho conferidos. O repositório principal continuou legível.
- Hardware via CIM: consulta recusada pelo Windows; registrar pelo editor ou ferramenta local antes de publicar FPS. Gate C0: **aberto**.
