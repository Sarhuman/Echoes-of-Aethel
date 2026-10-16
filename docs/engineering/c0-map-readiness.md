# C0-01 — prontidão dos 12 mapas para produção 3D

**Auditoria documental em 15/10/2026. Estado: aberta.** Comparei o [atlas de pré-produção](../design/campaign-world-atlas-3d.md), o [GDD 04](../GDD/04_mapas_e_quests.md) e o [registro C0](campaign-content-ledger.md). Todos os 12 mapas têm nome, percurso proposto, encontro final, ambiente e consequência descritos. Isso comprova cobertura narrativa inicial, **não** planta em escala, nível Unreal ou aprovação editorial. Nenhum mapa está pronto para produção jogável pelo critério abaixo.

Os blockouts estáticos [da Cidadela](c0-citadel-blockout.md) e [do Bosque](c0-forest-blockout.md) são amostras experimentais de circulação, com 21 e 32 atores técnicos recarregados no Unreal. Cidadela é hub fora dos 12 mapas; Bosque ainda não tem escala, câmera, interiores, colisão ou navegação aprovados. Estes níveis não promovem nenhuma linha abaixo e não fecham C0-07 nem o gate.

## Critério de pronto por mapa

Uma ficha só passa a `Pronto para blockout` quando registra, com revisão, (1) entrada, retorno e âncoras; (2) planta em metros com largura de rota, câmera e cápsula medidas; (3) portas, interiores e colisão estática/móvel; (4) encontro e telegraphs para os cinco heróis; (5) ID de quest, opções, evidências, itens e `reward_id` persistidos; (6) lista de modelos, rig, materiais, VFX e áudio com origem; (7) plano de testes de ida/volta, acessibilidade e desempenho. `Pronto para gameplay` só existe após o gate C0 global e implementação/testes próprios.

| Mapa | Contrato narrativo já localizado | Lacuna específica a resolver antes do blockout | Estado |
|---|---|---|---|
| 01 `forest_01` | `act1.spy_hunt`, Garras de Musgo; primeira rota mais detalhada | Poço/cercas com colisão e acesso real à casa/porão; rota e arena medidas; Espectro de Folhas sem ficha | Proposta |
| 02 `swamp_02` | `act1.desperate_alchemist`, Matriarca das Águas | Profundidade/veneno/antídoto, passarelas estáveis e leitura do laboratório antes da escolha | Proposta |
| 03 `temple_03` | `act1.guardian_roots`, Silvano | Fases e telegraphs do boss; arquivo de selos acessível; raízes não podem bloquear retorno | Proposta |
| 04 `dunes_04` | `act2.lost_caravan`, Guerreiro de Vidro | Visibilidade na tempestade, manifesto no vagão, variantes do memorial e apresentação dos nômades | Proposta |
| 05 `valley_05` | `act2.flame_ritual`, Executor de Bronze | Custo temporário comunicado, largura de pontes para invocações e câmara de inscrições | Proposta |
| 06 `crypt_06` | `act2.ossuary`, Malignus | Fases/silhueta do boss, informação das almas e separação da dungeon opcional de Namar | Proposta |
| 07 `sewers_07` | `act3.purification_valve`, Mãe das Proles | Altura/canais, natação, retorno sem obstrução e consequências visíveis no Refúgio | Proposta |
| 08 `foundry_08` | `act3.energy_core`, Autômato Forjador | NavMesh/colisão de esteiras e pistões; distinguir inimigo comum do subchefe | Proposta |
| 09 `abyss_09` | `act3.last_hunger`, Gorgoroth | Fases do boss, leitura das vítimas e alternativa de efeitos reduzidos | Proposta |
| 10 `gardens_10` | `act4.judgment_throne`, Arconte de Prata | Parapeitos e rota de retorno; custo da escolha na UI; iconografia distinta | Proposta |
| 11 `bridge_11` | `act4.last_gate`, Guardião dos Portões | Espaço de esquiva dos cinco heróis/invocações; ponte móvel somente com rota segura | Proposta |
| 12 `heart_12` | `act4.heart_fate`, Vael | Fases, câmera, música, epílogos e matriz de flags; arquivo de revelação antes da arena | Proposta |

## Primeira sequência de trabalho

1. **C0-07, medição descartável:** definir cápsulas provisórias dos cinco heróis, câmera e larguras mínimas numa arena técnica sem quest, loot ou loop. Registrar valores medidos e hardware; não promover a arena a mapa de campanha.
2. Aplicar as medidas às 12 plantas e marcar explicitamente cada valor `MEDIDO` ou `EXPERIMENTAL`.
3. Catalogar portas, interiores, recipientes, recompensas e IDs persistidos; manter sem loot os locais cuja recompensa não esteja definida.
4. Revisar com GDD 04/07/08 as consequências e apresentações de cada opção antes de aprovar plantas ou assets.
5. Registrar aprovação editorial e evidência por mapa nesta matriz. Uma linha marcada `Proposta` não fecha o gate C0.

O Bastião e as 18 pranchas existentes são referências de conceito. Nenhuma prancha, descrição ou compilação do módulo C++ substitui escala, modelo, rig, navegação e teste em câmera de jogo.
