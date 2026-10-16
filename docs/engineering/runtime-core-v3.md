# Núcleo C# v3 — quatro kits, loot e catálogo de encontros

Implementação independente de engine, 16/10/2026. Complementa o núcleo v2; **não é um cliente Unity jogável**. Dados novos desta revisão são **NOVOS/EXPERIMENTAIS**, submetidos à revisão editorial no PR. C0/U0 permanecem abertos. Godot/Unreal e seus saves não foram alterados.

## O que pode ser executado

`ClassCombatService` usa o mesmo `ProgressionService.Run` e agregado/CAS existentes. Há quatro ativas, duas passivas e oito modificadores de especialização para cada uma das outras quatro classes. A1 gratuito; compras básicas, níveis e seis pontos de especialização continuam regidos pelos catálogos existentes. Não se adicionou quinta ativa ou tecla. A numeração A1–A4 é a do GDD, não uma mudança no mapeamento de input do cliente antigo.

| Classe | Regras portadas do GDD 01 |
|---|---|
| Tecelão | Projétil no primeiro alvo, geada 25%/boss 10%, brasas em área, barreira substituível, Fratura com CD próprio, regeneração extra fora de combate |
| Sentinela | Adaga, ocultação quebrada por ataque/dano sem apagar memória da IA, uma armadilha, sangramento, ruptura com lentidão, abertura de sombra consumida no lançamento, bônus contra sangramento próprio |
| Invocadora | Projétil, um golem e uma fera, snapshots de INT/foco/nível, defesa do golem, intervalos próprios, morte/dano/expiração, veneno por alvo sem duplicar na reentrada, regeneração extra única fora de combate |
| Inquisidor | Maça física, martelo sagrado, queimadura com tick adicional da passiva, autocura sem crítico/arma/consumo com HP cheio, cone com antecipação de 700 ms, bônus direto contra mortos-vivos |

Baseline de poderes/custos/CD/durações: `CombatKitCatalog.cs`, portado do GDD 01. Fórmulas: `CombatMath` e `CharacterRules`, inclusive MDEF, chance crítica até 25% e multiplicador até 2×. Crítico é sorteado uma vez por acerto direto pelo adaptador confiável, de forma estável por ação/alvo; ticks e invocações não herdam crítico. Snapshots preservam dano, crítico, modificadores e parâmetros após troca de equipamento ou respec.

**Tempos NOVOS/EXPERIMENTAIS:** tolerância de acerto de 250 ms após antecipação; trava global de ação de antecipação +150 ms. Armadilha fica disponível por 8 s; jardim por 3 s. Esses tempos são contratos para o adaptador físico testar, sem alegar alcance ou colisão implementados.

DoT usa P_tick + W + 2×atributo + 2×(nível−1), uma vez/segundo; mesmo tipo/lançador renova sem acumular. Esta simulação tem **um protagonista/lançador**; limite de três lançadores do GDD não tem aplicação entre múltiplos jogadores neste cliente offline. Todos os ticks de um frame longo são drenados uma vez. Efeitos pendentes de dano contínuo bloqueiam regeneração; o adaptador deve resolver ticks antes de considerar o personagem fora de combate. Não acumular ataques perdidos de invocações como burst após um frame longo.

## Especializações novas: parâmetros explícitos

IDs preservados de `SpecializationCatalog`; parâmetros centralizados em `KitSpecializationBalance.cs`. Cada coluna é o efeito do tier correspondente; requisitos/cadeia 5/10/20/30 e limite de seis investimentos continuam iguais. Modificadores são aditivos e respeitam tetos. Só afetam as habilidades compradas; não concedem nós básicos automaticamente.

| Caminho | t1 | t2 | t3 | t4 |
|---|---|---|---|---|
| rune_weaver.control | +5 pp lentidão em comuns | Geada +1 s | Geada −3 MP | Brasas +1 tick |
| rune_weaver.burst | Brasas +10% direto | Brasas −4 MP | A1 +10% | Brasas +15% adicional |
| shadow_sentinel.precision | A1 +10% | Ruptura −3 MP | Ruptura +10% | Sangue Frio +10 pp |
| shadow_sentinel.ambush | Ocultação +1 s | Armadilha −3 MP | Sangramento +1 tick | Última Sombra +15 pp |
| thorn_summoner.command | Invocações +20% HP | Golem −4 MP | Invocações +15% dano | Invocações +2 s |
| thorn_summoner.sacrifice | Jardim consome golem vivo: +25% por tick | Jardim −3 MP | Jardim pode consumir fera viva: +1 tick | Consumo deliberado pelo Jardim cura 10% HP máximo, uma vez por uso |
| sacred_flame_inquisitor.protection | Cura +10% | Cura −4 MP | Barreira de 20% da cura efetiva, 3 s | Barreira passa a 40% da cura efetiva |
| sacred_flame_inquisitor.judgment | Martelo +10% direto | Veredito −4 MP | Veredito +10% direto | +10% direto sagrado contra mortos-vivos |

Sacrifício é consequência explícita de A3 com os nós escolhidos. Reinvocar/substituir/expirar/morrer não explode nem gera loot, cura ou sacrifício. O Jardim no tier 4 consome as invocações disponíveis elegíveis, cura uma vez e mantém seu CD; não existe comando de sacrifício independente. Bosses/subchefes mantêm 10% de lentidão mesmo com controle. Invocações existentes preservam seu snapshot quando há respec.

## Loot avançado e equipamento

`AdvancedLoot.cs` contém conteúdo **experimental v1**, separado dos 16 templates legados gerados de Godot. Doze templates de equipamento: machado, espada, adaga, cajado, maça, escudo, elmo, peitoral, luvas, botas, anel e amuleto. Nove slots, duas mãos no cajado, requisitos de classe/nível, duas instâncias distintas para anéis. Arcos/bestas e focos secundários futuros ficam fora do pool habilitado. Equipamento ocupa a grade existente e permanece nela quando equipado.

Dimensões experimentais: arma 1×3, peitoral 2×3, outros 1×1. Poder base de arma 8 + floor((item_level−1)/2), armadura base 2 nos slots de proteção. Faixa de item vem do encontro confirmado, entre 1 e 30; requisito = nível de item neste baseline. Não é uma curva balanceada. Cada classe tem A1 gratuito mesmo sem arma; não inventar um drop obrigatório de cajado/adaga/maça para começar.

Cinco raridades, contagens prefixo/sufixo 0/0, 1/0, 1/1, 2/1, 2/2. Sorteio com pesos inteiros, sem reposição de grupo/ID, sem reroll infinito. Tiers 1 nos níveis 1–10, 2 em 11–20 e 3 em 21–30; valores mínimos/máximos abaixo multiplicados pelo tier.

| Afixo | Faixa tier 1 | Peso | Restrição |
|---|---:|---:|---|
| Prefixos STR, DEX, INT, FTH | 1–3 | 100 cada | Todos os templates habilitados |
| Prefixo dano | 100–500 bp | 50 | Teto efetivo +100% |
| Prefixo ward | 1–5 | 100 | Soma em MDEF |
| Prefixo crítico | 100–250 bp | 50 | Teto efetivo 25% |
| Sufixo VIT | 1–3 | 100 | Atributo efetivo até 99 |
| Sufixo armadura | 1–5 | 100 | Soma antes de DEF |
| Sufixo cura | 100–500 bp | 50 | Teto efetivo +50% |
| Sufixo resistência | 100–500 bp | 100 | Bônus uniforme aos cinco tipos, teto 75% |
| Sufixo ferocidade | 100–500 bp | 25 | Só Raro/Épico/Lendário; multiplicador total até 2× |

Lendários têm propriedade fixa por família: arma `unique.tempered_edge` (+5% dano direto do herói); anel/amuleto `unique.merciful_ember` (+5% cura); demais `unique.steadfast_ward` (+5 ward). Cada grupo aplica uma vez mesmo com dois itens; sem procs recursivos, scripts ou asset paths no save. São primeiros efeitos experimentais, não catálogo final de lendários autorais. Dados persistidos são template versionado + roll; estatísticas são calculadas uma vez, não duplicadas de uma projeção `Stats`.

Probabilidades/quantidades e raridades por categoria reproduzem GDD 03. Colunas independentes; 80% do grupo de equipamento favorece classe, 20% outras famílias já implementadas. Boss garante equipamento, ouro, consumível, material e essência. Materiais/essências usam `exp.material`/`exp.essence` genéricos neste incremento; afinidades e receitas de crafting ainda não têm catálogo econômico final. Valores temáticos do GDD não foram convertidos silenciosamente em itens vendáveis.

`EncounterRewardService.Claim` é a entrada de produção: valida vitória/nível, gera roll estável por spawn, concede loot/XP/ouro, registra confronto terminal e receita/fragmento na mesma transação. RNG é injetado pelo adaptador de encontro com seed persistida; a UI nunca fornece seed ou drop. Falha antes do commit permite retry com o mesmo resultado; sucesso/reload/retry não sorteiam novamente. `GameplayService.AwardDefeat` virou interno, somente compatibilidade do harness anterior.

Primeira vitória de Silvano/Malignus/Gorgoroth/Vael concede receitas canônicas e fragmentos únicos. Vael é persistido antes da escolha final. Dungeons opcionais garantem Raro compatível na primeira vitória; não concedem fragmento. Não há farm/reentrada automática de confrontos terminais, crafting ou endgame. A reserva deve ter quatro vagas antes de autorizar novo encontro (pior caso: equipamento + três colunas de itens); bolsa livre pode receber a recompensa. Reserva cheia nunca destrói loot ou concede parcialmente XP/receita.

## Catálogo completo deste escopo

[encounter-catalog.json](encounter-catalog.json) é a fonte, com 51 entradas: 33 comuns da campanha, 12 finais de mapa, quatro elites opcionais e dois chefes opcionais. O gerador confere cobertura contra o manifesto e publica C# determinístico. A flag `optional` impede adicionar dungeons à cadeia obrigatória.

Todos os novos IDs `exp.*` estão explicitamente propostos nesta revisão, não derivados em runtime de nomes/nós. Preservados `ethereal_wolf`, `blind_cultist`, `moss_claws` e seus XP 10/12/200. Demais valores experimentais: XP=(10+2×(nível máximo−1))×categoria, multiplicadores 1/3/8/20; HP=(40+15×nível máximo)×categoria; DEF=5+2×nível máximo; MDEF=3+nível máximo; poder=5+2×nível máximo. Antecipação base 600 ms em comuns, 1000 ms em demais categorias. Campos de comportamento melee/ranged/area são contratos de catálogo, **não IA executável ou fases finais de boss**. A tabela é baseline conservadora para simular a economia; não introduz autoescala por nível do jogador.

Dungeons: `exp.dungeon.ash_crown` nível 10–12, acesso pelas Dunas; `exp.dungeon.lost_names` nível 14–16, acesso pela Cripta. Seus quatro elites e nomes de função também são propostas. Acesso exige região principal desbloqueada, nunca conclusão opcional para passar ao próximo mapa.

## Save, eventos e integração obrigatória

Schema **3** adiciona `Kit` e `Loot`. Migração 2→3 preserva XP, HP/MP, itens/IDs/posições, ouro, escolhas, cooldowns do Bastião e recibos; adiciona slots vazios e estados novos. Migração 1→2→3 continua suportada. Leitura não regrava arquivo; primeiro commit mantém `.bak` original. Não converte Godot/Unreal. Derrotas legadas já confirmadas não ganham uma segunda recompensa retroativa.

O núcleo não é dono da vida/posição de todos os inimigos. Acertos, alcance, alvo vivo, linha de visão, esquiva, dano recebido e RNG crítico exigem adaptador de simulação confiável (`IClassCombatAccess`), que ainda será integrado à Unity. **Não ligar esses métodos diretamente a botões da UI como evidência.**

Dano calculado entra numa **outbox persistida** (`Kit.Pending`, até 512). Cada emissão tem ID estável. O adaptador deve aplicar uma vez por ID, persistir esse recibo no estado do ator/encontro e só então confirmar `AcknowledgeEmission`; após crash, drenar pendências sem duplicar. Emissões retornadas em `ClassCombatResult` são notificação pós-commit, não segunda concessão de dano. Sem esse consumidor idempotente e persistência de atores, não anunciar combate completo ou crash safety de inimigos. Antes de transição/reset, drenar efeitos confirmados e cancelar áreas/invocações via fluxo de hub/morte; decidir destino do encontro na integração, sem apagar fatos já emitidos.

Não persistir a cada frame: relógio e comandos são API de domínio, a cadência real precisa do adaptador/benchmark. Limites: arquivo 1 MiB, 8.192 derrotas, 512 recibos, 32 ações, 128 efeitos, duas invocações, 100 instâncias em reserva. Capacidade esgotada recusa transação e exige coleta/ack; não compacta história de vitória silenciosamente.

## Validação e próximos aceites

Validação local executada em Mono 6.8/C# 7.2: 737 verificações do runtime anterior + 1.436 novas + 22.046 do agregado; memória: 184.377 verificações, incluindo 4.096 rotas. Cinco geradores e 13 testes Python passaram. Amostragem de loot: 5.000 seeds de comuns, tolerância absoluta de três pontos percentuais, além de 1.200 rolls das quatro categorias e limites determinísticos. Isso é detecção de regressão, não aprovação de economia ou estatística de produção. Windows/.NET 8 e Unity não foram executados localmente.

Reproduzir com `bash tools/run_campaign_tests.sh`, `bash tools/run_world_memory_tests.sh`, cinco geradores e testes Python. O harness novo cobre kits/passivas, controle de bosses, ticks longos, outbox/reload/ack, dano e morte de invocação, cura, antecipação, amostragem determinística de loot, rota de 12 mapas, dungeons, receitas/fragmentos e migração 2→3. Os totais incluem laços e não medem cobertura de gameplay.

Ainda exigem trabalho: consumidor de dano/atores na engine, física/IA/navegação, telegraphs/fases autorais dos 14 confrontos, controles e menus, diálogos/cenas, localização final, crafting e economia temática, mapa/modelos/áudio, build Windows, performance no Xeon/RX 580/16 GB e playtest/balanceamento. Portanto, este incremento fecha regras testáveis dos três módulos solicitados, não declara que só falta integração visual ao jogo inteiro.
