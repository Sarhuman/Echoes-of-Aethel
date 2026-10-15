# Hubs e histórias secundárias — pré-produção 3D

**Estado:** proposta `NOVO/EXPERIMENTAL` onde indicado. Os quatro hubs seguros, as três secundárias narrativas e as escolhas principais vêm de [GDD 04](../GDD/04_mapas_e_quests.md) e do [resumo da campanha](campaign.md). Namar e Gorvak são propostas em [elenco e dungeons](playable-cast-and-side-dungeons.md). O [registro C0](../engineering/campaign-content-ledger.md) acompanha IDs/recompensas ainda ausentes. Este documento planeja contexto e circulação, sem criar zonas, NPCs ou quests no runtime.

## Quatro lugares de retorno

Cada hub tem um **núcleo de chegada**, um **serviço de cura/preparação**, um **ponto de save/inventário**, conversa e saída em ambos os sentidos. A chegada nunca coloca o personagem atrás de balcão, em terreno hostil ou dentro de outro ator. A câmera identifica a saída antes de abrir UI. A Cidadela mantém o cristal/serviços em todos os estágios; outros hubs seguem segurança do GDD 04. Checkpoint técnico e autorização de viagem precisam respeitar o save, sem derivar da posição do portal.

| Hub | Percurso de produção proposto | Personagens e contexto |
|---|---|---|
| Cidadela `citadel` | Portão leste → praça/cristal → conselho/arquivo; ramais curtos para Lyra, Sera e Bram, com oficina acessível proposta na [primeira rota](first-route-3d.md) | Irian, Bram, Lyra, Maera e Sera presentes e alcançáveis. Após cada ato, compor estágio + flags concretas do GDD 08. Memorial, água isolada, filas e máquinas são variantes, nunca bloqueios ao serviço. |
| Acampamento das Dunas `dunes_camp` | Portão de abrigo → área de água e provisões → conselho de caravana → saída para `dunes_04`; posição de retorno reconhecível por marco de ossada | Equipe de caravana e sobreviventes por `act2.lost_caravan`; vendedor local ainda sem nome/ID. Tendas e tecido mostram vento sem ocultar placa de saída. Serviço essencial continua mesmo se a carga for perdida. |
| Refúgio Subterrâneo `underground_refuge` | Escada de chegada → plataforma seca de triagem → arquivo hidráulico → saída para `sewers_07`; canais mantêm guarda física | Trabalhadores e comunidades inferiores precisam de voz própria antes de `act3.purification_valve`; vendedor/curador locais ainda sem nome/ID. Fluxo da água sinaliza estado da quest, não aplica dano no hub. |
| Posto do Limiar `liminal_post` | Entrada protegida → arquivo da genealogia → preparação/retorno → saída para `gardens_10` e caminho final | Irian ou registros equivalentes fornecem a prova de linhagem do GDD 07 antes de Vael. Responsável por suprimentos ainda sem nome/ID. O posto não antecipa o epílogo nem força o fragmento da Cidadela a sair do hub. |

**Falta definir:** planta em escala, quem opera serviços nos hubs II–IV, diálogos por estágio/flag, ponto de save e regras de teleporte, variações de cenário, iluminação/áudio e IDs técnicos de atores novos. Os `zone_id` acima já existem em `WorldSaveSchema.ZONES` Godot; não renomear em proposta visual. [Comércio regional](regional-commerce-3d.md) define a política de vitrine, ainda sem novos preços ou templates.

## Três secundárias narrativas previstas

Estas missões ampliam pessoas e preparação, sem substituir a decisão obrigatória do mapa. **Não há `quest_id`, recompensa ou posição final aprovados**; qualquer texto de missão, entrada e item abaixo é proposta de produção. Escolhas secundárias usam seu próprio estado e não incrementam `ritual_charge`, que pertence às doze decisões principais.

| História prevista | Onde se encaixa e o que o jogador descobre | Resolução já indicada no resumo | Conteúdo e recompensa a fechar |
|---|---|---|---|
| **O Julgamento do Cultista** (ato I) | Após encontrar provas cultistas no Bosque, mas com grupo de pessoas **diferente** dos acusados de `act1.spy_hunt`. Uma testemunha aponta crianças desaparecidas; o cultista conhece a rota. | Poupar permite resgatar filhos, mas espalha contaminação; executar impede o vetor, mas abandona o resgate e alimenta a maldição. | Definir pista e testemunha, local seguro da conversa, condição de resgate, resposta de Maera/Irian, `quest_id`, XP/ouro/item fixos e flag persistente. Não reusar a escolha da aldeia. |
| **A Água da Vida** (ato II) | Na rota das Dunas/Vale, o poço cura soldados, mas se liga a um selo e altera sua devoção. A inscrição dá informação suficiente antes da decisão. | Canalizar cura e cria fanatismo; **selar e inutilizar a saída** preserva a barreira, mas perde a cura. | Definir localização exata, personagens afetados, leitura visual do selo, resposta de Lyra/acampamento e recompensa que ajude no próximo confronto sem tornar a missão obrigatória. |
| **A Aliança com o Traidor** (ato III) | Uma pessoa aprisionada oferece informação sobre máquinas/linhagem enquanto sua rede de almas continua ativa. Evidência adicional verifica ao menos parte da oferta. | Libertar o general fornece informação e fortalece sua rede; matar elimina a rede, mas libera uma onda que altera encontros. | Definir zona, nome/ID do general, encontros modificados, pista alternativa da linhagem para quem não aceita a aliança, recompensa e retorno seguro. Não reduzir permanentemente XP/loot. |

Não introduzir um dilema oculto que invalide o texto da opção. O diário mostra risco conhecido, local, progresso e recompensa prometida; custo futuro só aparece após o evento observado. Cada entrega de recompensa requer `reward_id` único por campanha, bolsa cheia/reserva, retry idempotente e preço/XP simulado. Secundária abandonada não fecha o caminho da campanha. Retorno ao contratante pode resolver narrativa mesmo quando a escolha já foi confirmada; não rolar loot na conversa.

## Duas dungeons opcionais propostas

| Dungeon | Entrada e NPC sugeridos | Escopo de história e retirada |
|---|---|---|
| **A Coroa de Sucata** | Entrada autoral em `dunes_04`; Sera menciona caravanas desaparecidas, sem presumir que opere a loja local | Goblins e orcs da Coligação das Cinzas têm motivo e cultura além de “inimigos por raça”; Gorvak é líder subchefe. Trilha lateral → fortaleza → arena → volta à âncora salva. Uma rota de retirada existe antes da arena. |
| **O Livro dos Nomes Perdidos** | Entrada autoral em `crypt_06`; Irian oferece registros, sem bloquear o fragmento de Malignus | Namar, rei-lich original, conserva nomes nas placas ósseas. Galeria de inscrições → salas de ecos → arena → retorno seguro. Informação obrigatória da quest principal não fica dentro desta dungeon. |

As dungeons ficam **fora** dos 12 mapas. São mais exigentes, opcionais e não fornecem fragmentos da campanha. O [contrato de entrada e recompensa](../engineering/optional-dungeon-contract.md) exige âncora validada, bilhete de retorno, seed persistida se houver variação, categoria de loot de subchefe e prêmio de primeira conclusão com ID único. O catálogo de inimigos, salas, diálogos e itens ainda precisa de aprovação; não prometer um raro/lendário específico antes da simulação. Primeiro layout é autoral fixo.

## Entregáveis de conteúdo antes de gameplay

Para cada hub: planta com pontos de chegada/saída e serviços, retratos/turnarounds de atores locais, textos de chegada/retorno/flag, três estados visuais relevantes e lista de props/música original. Para cada secundária: `quest_id`, localização, gatilho, pistas, escolha e consequências, NPC, encontro, recompensa, `reward_id`, texto do diário e condições de repetição. Para cada dungeon: planta fixa, roster, arena, loot/XP simulado, bilhete e retirada. Todos os novos IDs só entram no catálogo após revisão; esta ficha não fecha o gate C0 sozinha.
