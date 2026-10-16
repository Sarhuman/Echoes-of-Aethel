# Índice de produção da campanha

**Pré-produção 3D:** [atlas dos doze mapas](../design/campaign-world-atlas-3d.md), [elenco/bestiário](../design/campaign-cast-and-bestiary-3d.md) e [registro de conteúdo](campaign-content-ledger.md) detalham propostas, contratos e lacunas sem criar runtime. A [ordem atual](unreal-execution-plan.md) exige o gate de conteúdo C0 antes da parte jogável Unreal.

[Hubs e secundárias](../design/hubs-and-side-stories-3d.md) detalham circulação, serviços e histórias opcionais. [Comércio regional](../design/regional-commerce-3d.md) propõe ofertas comuns, por classe e por ato; os novos itens/preços ainda não são catálogo aprovado.

As quatro ativas e duas passivas de cada classe estão no GDD 01; o [catálogo de produção das habilidades](../design/ability-production-catalog-3d.md) lista os assets e a leitura 3D pendentes.

Este índice evita confundir **conteúdo especificado** com mapas 3D construídos. A fonte detalhada das doze linhas é o [GDD 04](../GDD/04_mapas_e_quests.md); roteiro e variantes estão no [GDD 07](../GDD/07_roteiro_e_dialogos.md). IDs são preservados entre engines. A [primeira rota](../design/first-route-3d.md) é a única com IDs e conteúdo de recipientes propostos; o atlas sugere interiores para os outros mapas sem lhes atribuir tesouros. A [ficha do elenco e das dungeons secundárias](../design/playable-cast-and-side-dungeons.md) distingue cinco heróis especificados, NPCs e duas quests opcionais `NOVO/EXPERIMENTAL`, fora da contagem dos doze mapas. O [contrato de dungeons opcionais](optional-dungeon-contract.md) define entrada, diário, retorno e recompensa para a futura implementação.

O [manifesto da rota](campaign-route-manifest.json) oferece os 12 IDs de zona, quests, escolhas, encontros finais e dependências em formato legível por ferramentas. É derivado do GDD e deste índice, com estado `design_only_not_runtime`: não implementa desbloqueio nem substitui o texto narrativo. Para detectar divergências, execute `python tools/validate_campaign_route.py` da raiz; após uma mudança deliberada nas fontes, revise a diferença antes de regenerar com `--write`. IDs técnicos ainda ausentes permanecem `null`.

| Ato/mapa | Zona | Quest principal | Confronto | Estado de produção |
|---|---|---|---|---|
| I/01 Bosque das Névoas | `forest_01` | `act1.spy_hunt` | Garras de Musgo, subchefe | Jogável em Godot; ficha 3D e interiores propostos |
| I/02 Pântano dos Lamentantes | `swamp_02` | `act1.desperate_alchemist` | Matriarca das Águas, subchefe | GDD; sem mapa 3D |
| I/03 Portal do Julgamento | `temple_03` | `act1.guardian_roots` | Silvano, boss de ato | GDD; sem mapa 3D |
| II/04 Dunas de Sal | `dunes_04` | `act2.lost_caravan` | Guerreiro de Vidro, subchefe | GDD; sem mapa 3D |
| II/05 Vale dos Sacrifícios | `valley_05` | `act2.flame_ritual` | Executor de Bronze, subchefe | GDD; sem mapa 3D |
| II/06 Cripta de Ossos | `crypt_06` | `act2.ossuary` | Malignus, boss de ato | GDD; sem mapa 3D |
| III/07 Esgotos de Éter | `sewers_07` | `act3.purification_valve` | Mãe das Proles, subchefe | GDD; sem mapa 3D |
| III/08 Fundição Proibida | `foundry_08` | `act3.energy_core` | Autômato Forjador, subchefe | GDD; sem mapa 3D |
| III/09 Abismo da Voracidade | `abyss_09` | `act3.last_hunger` | Gorgoroth, boss de ato | GDD; sem mapa 3D |
| IV/10 Jardins Suspensos de Éter | `gardens_10` | `act4.judgment_throne` | Arconte de Prata, subchefe | GDD; sem mapa 3D |
| IV/11 Ponte dos Lamentos | `bridge_11` | `act4.last_gate` | Guardião dos Portões, subchefe | GDD; sem mapa 3D |
| IV/12 Coração de Éter | `heart_12` | `act4.heart_fate` | Vael, boss final | GDD; sem mapa 3D |

**Hub:** Cidadela é segura e checkpoint da primeira rota. Acampamento das Dunas, Refúgio Subterrâneo e Posto do Limiar são hubs de atos posteriores, previstos no GDD. Bram, Irian, Lyra, Maera e Sera têm papéis e falas em variantes; somente o primeiro estágio tem runtime. As cinco classes têm identidade e habilidades no GDD 01; somente Bastião tem runtime. Mobs comuns por mapa constam em GDD 04, não equivalem a modelos/IA prontos.

## Ficha obrigatória antes de construir qualquer outro mapa

1. Percurso, entrada/saída, checkpoint autorizado, câmera, piso transitável, obstáculos, arena e rota de retorno.
2. NPCs, falas, pistas, escolha e consequências persistidas com IDs do GDD; não revelar custo oculto na opção.
3. Encontros comuns, subchefe/boss, telegraphs, navegação e limites de perseguição.
4. Interiores acessíveis com porta de ida/volta, função, leitura visual, conteúdo e estado de respawn. Casas decorativas ficam identificadas como tal.
5. Recipientes com `reward_id`, catálogo de item, política de coleta, bolsa cheia e persistência; nenhum loot sem origem ou gerado pela UI.
6. Lista de modelos, animações, materiais, VFX, áudio e proveniência. Medir desempenho e acessibilidade na build Windows.

Um mapa só passa de **GDD** para **blockout** quando tiver essa ficha e um percurso testável. Só passa a **jogável** quando entrada/saída, combate, escolha, recompensa e reload forem verificados. Arte final é estado separado, não sinônimo de jogável.
