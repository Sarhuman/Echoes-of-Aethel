# Valores experimentais da primeira fatia

Data: 13/10/2026 · Decisão D4 de [decisions-first-slice.md](decisions-first-slice.md).

**Legenda de origem:** `GDD` = valor já definido em documento de design (ainda experimental pelo próprio GDD); `PROTO` = valor do protótipo atual; `NOVO` = proposto agora porque nenhum documento define; **todos** são `EXPERIMENTAL` e dependem de playtest. Código lê estes valores de `Resource`s em `content/`, nunca de literais espalhados.

## 1. Bastião (nível 1)

| Item | Valor | Origem |
|---|---|---|
| STR / DEX / INT / FTH / VIT | 12 / 6 / 3 / 5 / 14 | GDD 01 |
| HP máx = `60 + 8×VIT + 6×(L−1)` | 172 | GDD 01 |
| MP máx = `30 + 4×INT + 2×FTH + 3×(L−1)` | 52 | GDD 01 |
| DEF = `armadura + floor(VIT/2) + 2×(L−1)` | 9 (com `worn_tunic` armadura 2) | GDD 01 + NOVO |
| MDEF = `ward + floor((INT+FTH)/3) + (L−1)` | 2 (ward 0) | GDD 01 |
| Crítico | `min(25%, 5% + 0,25%×DEX)` = 6,5%; multiplicador 1,5 | GDD 01 |
| Stamina máx / custo do dash / cooldown / duração | 100 / 25 / 1,2 s / 0,2 s | PROTO / decisões de produção |
| Invulnerabilidade do dash | 120 ms (tempo de simulação) | decisões de produção |
| Regeneração de MP | `1 + floor(INT/10)` = 1 MP/s, só fora de combate após 3 s sem gasto | GDD 05 |
| Regeneração de stamina | 12 /s | PROTO |
| Velocidade / dash | 150 / 400 px/s | PROTO |

## 2. Habilidades do Bastião

Dano pela fórmula de D3 com `W = 8` (`training_axe`), `A = STR = 12`.

| Habilidade | Slot | MP | P | Prep / ativo / recup. | CD | Efeito | Origem |
|---|---|---:|---:|---|---:|---|---|
| A1 Golpe de Machado | `attack_basic` | 0 | 10 | 150 / 80 / 250 ms | 0,9 s | arco frontal curto (alcance 56) | GDD + PROTO |
| A3 Impacto do Baluarte | `skill_1` | 12 | 18 | 350 / 80 / 370 ms | 7 s | interrompe comuns 0,5 s; boss sem stun (alcance 64) | GDD + PROTO |
| A2 Guarda do Crepúsculo | `skill_2` | 8 | — | instantânea | 6 s | −40% dano frontal por 2 s; costas desprotegidas | GDD |
| A4 Fenda de Aço | `skill_3` | 20 | 30 | 600 / 100 / 400 ms | 9 s | linha curta, antecipação 0,6 s | GDD (prep) + NOVO (ativo/recup.) |

Dano calculado (sem crítico): básico 42 base · Impacto 50 · Fenda 62 antes da defesa.

## 3. Inimigos

Dano do inimigo ao jogador: `final = max(1, floor(P × 100 / (100 + DEF_ou_MDEF_do_jogador)))`. `P` físico usa DEF 9; `P` arcano usa MDEF 2. Todos NOVO, exceto telegraphs (GDD `prologue-and-act-one.md`).

| `enemy_id` | HP | DEF | P | Tipo | Velocidade | Detecção | Alcance | Telegraph (GDD) | Recuperação (GDD) | Cooldown |
|---|---:|---:|---:|---|---:|---:|---:|---|---|---:|
| `ethereal_wolf` | 60 | 5 | 18 | físico (investida) | 120 | 200 | 40 | uivo + faixa 0,7 s | 0,9 s | 1,5 s |
| `blind_cultist` | 45 | 3 | 14 | arcano (projétil) | 70 | 220 | 160 | selo circular 1 s | 1,2 s | 2,0 s |
| `moss_claws` (sub-boss) | 800 | 15 | ver §4 | físico | 80 | 400 | ver §4 | ver §4 | 1 s | ver §4 |

Resultados esperados (conferidos por cálculo): lobo e cultista caem em 2 acertos de A1; jogador sobrevive a 11 investidas de lobo ou 14 projéteis de cultista; Garras de Musgo cai em ~23 acertos de A1 ou ~16 de A4.

## 4. Garras de Musgo

| Ataque | P | Telegraph (GDD) | Recuperação (GDD) | Dano ao jogador (DEF 9) | Restrição (GDD) |
|---|---:|---|---|---:|---|
| Varredura em arco | 32 | 0,9 s | 1 s | 29 | — |
| Investida reta | 40 | 1,1 s | 1 s | 36 | pilares bloqueiam |
| Raízes em duas áreas alternadas (<50% HP) | 24 | 1,2 s | 1 s | 22 | corredor livre sempre; telegraphs não diminuem |

Sem stun. Intervalo mínimo entre ataques: 1,2 s. Onda de choque: **não implementar** (não definida).

## 5. XP e níveis

| Item | Valor | Origem |
|---|---|---|
| `xp = floor(base_xp × mult)`; mult comum 1 · elite 3 · sub-boss 8 · boss 20 | — | GDD 03 §3.4 |
| `base_xp` | lobo 10 · cultista 12 · Garras de Musgo 25 (→ 200) | NOVO |
| XP para o próximo nível | `100 + 50×(L−1)` | NOVO |
| Nível máximo | 30 | GDD 01 |
| Pontos por nível | 3 (acumulam como `unspent_points`; **sem tela para gastar** no slice) | GDD 01 + D4 |
| Ao subir de nível | HP/MP máx recalculados pelas fórmulas; **não cura** | GDD 01 |

## 6. Drops (GDD 03 §3)

Uma rolagem independente por coluna, uma vez por morte validada (`source_id` = ID de spawn).

| Categoria | Equipamento | Ouro (chance / qtd) | Consumível | Material | Essência |
|---|---:|---|---:|---:|---:|
| Comum | 12% | 60% / 1–5 | 15% | 5% | 0% |
| Sub-boss | 100% | 100% / 25–50 | 60% | 100% | 100% |

Raridade (condicionada ao equipamento; pesos inteiros somando 10.000):

| Categoria | Comum | Mágico | Raro | Épico |
|---|---:|---:|---:|---:|
| Comum | 7500 | 2500 | 0 | 0 |
| Sub-boss | 0 | 0 | 7500 | 2500 |

Escolha dentro do tipo (NOVO): consumível — `potion_health_small` 50% · `flask_aether_small` 30% · `antidote_flask` 20%. Material do lobo: `wolf_fang`; do cultista: `cultist_ash`; do sub-boss: `wolf_fang` ou `cultist_ash` 50/50. Essência: `root_essence`. Equipamento: template aleatório (RNG injetado) entre os da raridade sorteada, slot `main_hand` ou `chest` 50/50.

## 7. Inventário, peso e itens

| Item | Valor | Origem |
|---|---|---|
| Posições | **60** | GDD 03 §2.1 |
| Stack de consumível/material | 20 | GDD 03 §2.1 |
| Equipamento | 1 posição; equipado continua na bolsa (sem 2ª posição) | GDD 03 §2.1 |
| Ouro | campo inteiro separado, sem peso | GDD 03 §2.1 |
| Peso máximo | **60,0** | NOVO |
| Peso por unidade | consumível 0,5 · material 0,25 · `main_hand` 4,0 · `chest` 6,0 · quest **0 e ignora limites** | NOVO |
| Item de quest | nunca recusado por bolsa cheia | decisões de produção |

Consumíveis (GDD 03 §2.1 e GDD 05): `potion_health_small` restaura até 50 HP · `flask_aether_small` até 30 MP · `antidote_flask` remove veneno removível. Cooldown compartilhado vida/MP 5 s; antídoto 5 s próprio. Recurso cheio ou efeito inexistente **não consome** item nem inicia cooldown.

Preço (Lyra, `npc.alchemist`): Poção de Vida pequena 10 ouro · Frasco de Éter pequeno 12 ouro (GDD 05). Compra fora do slice mínimo é opcional; se implementada: débito e entrega na mesma transação, idempotente, só no hub, recusada sem consumir ouro.

## 8. Equipamento (sem afixos — D8)

Faixa de item do mapa 01: 1–3 (GDD 04); `requirement_level = 1`. Nomes são placeholders (NOVO).

| Slot | Comum | Mágico | Raro | Épico |
|---|---|---|---|---|
| `main_hand` | `iron_axe` W 9 | `runed_axe` W 11 | `bastion_axe` W 14 | `twilight_axe` W 18 |
| `chest` | `padded_vest` arm. 3 | `ironweave_cuirass` arm. 5 | `bastion_cuirass` arm. 8 | `twilight_cuirass` arm. 12 |
| Iniciais | `training_axe` W 8 | `worn_tunic` arm. 2 | — | — |

(`training_axe` e `worn_tunic` já vêm equipados; são Comum.)

## 9. Bruma e promoção

Fora do slice. Hub e saída inicial nunca geram elites (decisões de produção §3).

## 10. Como alterar um valor

1. Edite a linha aqui e o `.tres`/constante correspondente no mesmo PR.
2. Atualize os testes que dependem do valor e liste-os no PR.
3. Não escreva "balanceado" em PR, README ou changelog.
