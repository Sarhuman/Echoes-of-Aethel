# Registro de conteúdo para o gate C0

**Estado em 15/10/2026:** inventário de pré-produção; não é catálogo importável pela Unreal nem comprova asset pronto. Preserva IDs de [GDD 04](../GDD/04_mapas_e_quests.md), [roteiro 07](../GDD/07_roteiro_e_dialogos.md) e [loot 03](../GDD/03_sistema_de_loot.md). Layout e interiores são propostas do [atlas](../design/campaign-world-atlas-3d.md). Este registro acompanha **o que já tem contrato** e **o que precisa ser criado** antes da parte jogável.

As propostas de [hubs/secundárias](../design/hubs-and-side-stories-3d.md) e [comércio regional por classe](../design/regional-commerce-3d.md) aprofundam os pacotes pendentes sem declarar preços, itens ou IDs novos aprovados.

O [catálogo de produção das 30 habilidades](../design/ability-production-catalog-3d.md) liga os nós do GDD 01 a necessidades de animação, efeito, som e leitura 3D. Ainda não há IDs técnicos aprovados nem habilidades jogáveis Unreal.

## Missões principais e cenas

Cada uma das 12 quests tem duas opções canônicas; ambas liberam o próximo mapa após seu gate. O GDD 04 guarda textos, consequências ocultas e afinidades. A coluna “conteúdo de produção” lista evidência/encenação necessária, não uma nova condição de vitória. Pistas essenciais são acessíveis antes da escolha e não dependem de loot aleatório.

| Mapa / quest | Opções existentes | Marco de efeito tardio | Conteúdo de produção ainda necessário |
|---|---|---|---|
| 01 `act1.spy_hunt` | `judge_suspects` / `shelter_villagers` | `first_return_to_citadel` | Três pistas externas, moradores, cena de escolha, memorial ou água isolada; primeira rota já tem IDs de pistas |
| 02 `act1.desperate_alchemist` | `deliver_venom` / `destroy_research` | `act1_completion` | Pesquisador, cadernos e animais tratados, amostras/ala da enfermaria e falas de Lyra |
| 03 `act1.guardian_roots` | `seal_sap` / `drain_sap` | `act1_completion` | Raízes ligando casas/árvores, selos rachados, Silvano, fragmento e duas variantes de templo/hub |
| 04 `act2.lost_caravan` | `save_supplies` / `save_merchants` | `act2_completion` | Caravana, cordas e contagem de pessoas/carga, memorial ou filas; rota de Sera permanece funcional |
| 05 `act2.flame_ritual` | `offer_vitality` / `ritual_execution` | `enter_crypt_06` | Prisioneiro, sentença verificável, inscrições do altar e indicador do custo temporário conhecido |
| 06 `act2.ossuary` | `free_captives` / `reinforce_seal` | `act2_completion` | Almas reconhecíveis, rachaduras e corrente do selo, Malignus, fragmento, sonhos/vozes no hub |
| 07 `act3.purification_valve` | `drain_lower` / `reverse_flow` | `first_return_to_citadel` | Diagrama hidráulico, moradias inferiores, água de superfície, triagem/arquivo por flag |
| 08 `act3.energy_core` | `extract_core` / `stabilize_core` | `act3_completion` | Sinais de exploradores presos, núcleo, contenção e melhoria limitada de arma ou máquina segura |
| 09 `act3.last_hunger` | `break_bond` / `seal_creature` | `act3_completion` | Vítimas visíveis no vínculo, Gorgoroth, fragmento, reencontros ou placas sem nomes |
| 10 `act4.judgment_throne` | `grant_pardon` / `condemn_souls` | `act4_pre_final_checkpoint` | Origem da barreira, almas no pedestal, famílias/guardas e custo/benefício temporário claro |
| 11 `act4.last_gate` | `break_seal` / `channel_body` | `enter_heart_12` | Rachaduras, escolta, travessia, perda temporária de MP conhecida e duas variantes de vigília |
| 12 `act4.heart_fate` | `restore_order` / `embrace_ascension` | `campaign_epilogue` | Prova prévia de linhagem, Vael, fragmento, duas versões de epílogo moduladas pelas flags |

**Cenas já roteirizadas:** `prologue.last_bell`, `prologue.council`, os marcos de Silvano/Malignus/Gorgoroth, `transition.act2`, `transition.act3`, `transition.act4` e a revelação `liminal.archive.revelation` constam do GDD 07. Antes da produção, montar lista de tomadas, duração alvo, elenco, legenda, música original, versão sem voz e ponto de pulo para cada uma. Assistir e pular devem deixar o mesmo estado. Introduções de Matriarca, Guerreiro de Vidro e demais subchefes **não** têm roteiro cinematográfico aprovado; evitar fabricar uma sequência obrigatória para cada encontro.

## Hubs, personagens e serviços

| Hub | Contrato existente | Pacote de conteúdo pendente |
|---|---|---|
| Cidadela `citadel` | Segura em todos os estágios; checkpoint da primeira rota; Irian, Bram, Lyra, Maera e Sera | Plantas das variantes de estágio 0–4/epílogo, posições alcançáveis dos cinco, serviço substituto quando alguém se afasta, retratos/falas e mudanças por flags; nunca combate ou dano |
| Acampamento das Dunas `dunes_camp` | Hub seguro do ato II no GDD 04; ID já reservado no `WorldSaveSchema.ZONES` Godot | Planta, elenco local, serviços mínimos, viagem de ida/volta e visual de escassez/suprimentos; conteúdo 3D **NOVO/EXPERIMENTAL** |
| Refúgio Subterrâneo `underground_refuge` | Hub seguro do ato III no GDD 04; ID já reservado no schema | Planta, elenco local, preparação e contraste entre comunidade inferior e Cidadela; conteúdo 3D **NOVO/EXPERIMENTAL** |
| Posto do Limiar `liminal_post` | Hub seguro do ato IV no GDD 04; ID já reservado no schema | Planta, arquivo de linhagem, preparação antes de Vael e rota de retorno; conteúdo 3D **NOVO/EXPERIMENTAL** |

O [GDD 08](../GDD/08_dialogos_hub_degradacao.md) especifica a seleção de falas por fato observado, estágio e afinidade. A prioridade é epílogo → flag concreta visível → estágio/afinidade → fallback. Bram, Lyra e Irian têm variantes detalhadas; Maera e Sera precisam de cobertura equivalente para os estágios posteriores. O jogador não conhece consequências ocultas antes do marco. O `ritual_charge` cresce por decisões, nunca por tempo offline. Barracas decorativas podem fechar; cura, inventário, save e compra essencial seguem acessíveis.

## Comércio e preparação — o que é real e o que falta

| Assunto | Contrato atual | Pendência antes de produzir interface/lojas 3D |
|---|---|---|
| Compras do slice | `MerchantService` Godot vende `potion_health_small` por **10 ouro** e `flask_aether_small` por **12 ouro** na Cidadela; [GDD 05](../GDD/05_combate_e_hud.md) confirma valores experimentais e estoque de protótipo | Definir visual de Sera/Lyra e qual balcão apresenta a transação; preços podem mudar após simulação, nunca por escolha moral oculta |
| Compra autoritativa | Requisição idempotente, valida zona/ouro/bolsa; débito e item no mesmo commit; compra recusada não gasta ouro | Testar contrato equivalente Unreal antes de abrir qualquer loja; apresentação não concede item |
| Venda de itens | Sem tabela de preço ou contrato final de recompra no GDD 03; comércio amplo é futuro | Definir valor, quais itens são vendáveis, confirmação, desfazer/recuperar e proteção de únicos antes de habilitar botão “Vender” |
| Bram | Papel de ferreiro e preparação estabelecido; crafting amplo não implementado | Definir reparo/forja com custo e consumo de materiais somente após catálogo; receitas dos bosses são conhecimento, não itens vendáveis |
| Lyra | Cura e poções precisam continuar disponíveis em todas as variantes do hub | Definir estoque por ato só após economia; não vender veneno surpresa em consequência da história |
| Ouro | Campo inteiro separado, não stack; comuns, elites, subchefes e bosses têm probabilidades/quantidades iniciais no GDD 03 | Simular ouro obtido vs. custo de poções, mortes e serviços nos cinco protagonistas, por ato e por dificuldade de expedição |

## Drops, baús e recompensas

O [GDD 03](../GDD/03_sistema_de_loot.md) já define categorias comum/elite/subchefe/boss, cinco raridades, chances de equipamento/ouro/consumível/material/essência, slots, bolsa de 60 posições, stacks de 20 e quatro receitas/fragmentos únicos de Silvano, Malignus, Gorgoroth e Vael. Não criar tabelas locais alternativas para criaturas do [bestiário](../design/campaign-cast-and-bestiary-3d.md). Os itens temáticos do GDD 04 são intenção de catálogo; vários não têm template, peso ou valor ainda. `item_level` por zona é proposto, não nível obrigatório do jogador.

**Recipientes com conteúdo/ID definidos:** somente `reward.forest_01.abandoned_house.cache` (uma `potion_health_small`) e `reward.forest_01.root_cellar.cache` (um `antidote_flask`), ambos **NOVO/EXPERIMENTAL** no [manifesto da primeira rota](first-route-content.json). Para todos os interiores dos mapas 02–11, antes de acrescentar baú: definir `reward_id`, item/quantidade ou tabela validada, posição, condição de acesso, primeira coleta, bolsa cheia, reserva e prova de ida/volta. Casa acessível pode oferecer apenas história; não promete loot automaticamente. Nenhuma recompensa vem de fim de animação/cinemática.

**Boss de ato:** primeira vitória registra receita e fragmento únicos com item/XP/ouro da vitória. Repetir uma chamada ou recarregar retorna o resultado gravado, sem rolar de novo. **Dungeon opcional:** Namar e Gorvak são propostas fora dos doze mapas. Seguem a categoria subchefe, não garantem lendário. Contrato de entrada, quest e prêmio separado está em [dungeons opcionais](optional-dungeon-contract.md); IDs finais, recompensas fixas, NPCs e política de repetição ainda precisam de revisão.

## Quests secundárias e lacunas editoriais

O [resumo da campanha](../design/campaign.md) prevê **O Julgamento do Cultista** (ato I), **A Água da Vida** (II) e **A Aliança com o Traidor** (III). São histórias distintas das doze quests principais, mas ainda faltam `quest_id` estável, zona exata, acesso, pistas, personagens, escolha confirmável, valor de XP/ouro/item, `reward_id`, repetição e resposta no hub. As duas dungeons opcionais propostas acrescentam linhas de produção próprias; não criam automaticamente seis novos mapas de campanha, fragmentos ou portais aleatórios. O diário `J` mostra Campanha/Secundárias e apenas fatos observados.

## Estado do gate C0

| Pacote | Estado | Próxima definição verificável |
|---|---|---|
| IDs e opções das 12 quests | **GDD completo** | Reconciliar gatilhos, evidências e cenas com manifestos de conteúdo versionados |
| Percurso/ambientação dos 12 mapas | **Proposta de atlas** | Revisão editorial e plantas com escala após blockout não jogável |
| Cinco heróis, cinco NPCs, 33 mobs, 12 confrontos | **Índice de elenco** | Turnarounds, rigs, animações, telegraphs, áudio e fichas de cada ator |
| 30 habilidades das cinco classes | **Mecânica no GDD; apresentação proposta** | IDs, dados 3D, ícones, animações/VFX/SFX e revisão de input por classe |
| Hubs e serviços além da Cidadela | **Proposta de percurso/serviços; conteúdo pendente** | Plantas em escala, NPCs locais, variantes e testes de acesso |
| Itens, vendas e baús além da primeira rota | **Política de comércio proposta; catálogo pendente** | Templates, economia simulada e IDs de recompensa persistente |
| Secundárias e dungeons | **Fichas narrativas propostas** | IDs, contratos econômicos, plantas e aprovação editorial |
| Cinemáticas e trilha | **Roteiro/infraestrutura parcial** | Storyboards, áudio original, licenças, ensaio de pulo/retorno |

O gate C0 **não está concluído**. Esta tabela impede tratar a especificação de campanha ou a compilação C++ como mapa/ator/loot finalizado. A parte jogável Unreal só inicia quando a cobertura de conteúdo acordada no [plano](unreal-execution-plan.md) tiver revisão e evidência próprias.
