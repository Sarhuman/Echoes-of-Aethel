# Decisões da primeira fatia — resolução dos pontos pendentes

Data: 13/10/2026 · Escopo: Bastião · Cidadela · Bosque (`forest_01`) · Godot 4.6.

**Status:** decisões **propostas e adotadas como padrão de implementação a pedido do solicitante**. Foram derivadas dos documentos existentes (GDD 01, 03, 04, 05 e [decisões de produção](production-decisions.md)); onde os documentos não fixam valor, o valor é **EXPERIMENTAL** e está concentrado em [experimental-values.md](experimental-values.md). O Game Director pode revogar qualquer item: nesse caso, edite este arquivo primeiro e depois os cartões afetados.

Este documento complementa [production-decisions.md](production-decisions.md); em caso de conflito direto com ele, prevalece `production-decisions.md`, exceto nas **emendas** da seção 9, que existem justamente para ajustar o escopo.

## Resumo

| # | Tema | Decisão |
|---|---|---|
| D1 | Checkpoint do Bosque | Único checkpoint do slice = `citadel`. **Sem** alterar `WorldSaveSchema.ZONES` |
| D2 | Slots de habilidade | `attack_basic`=Golpe de Machado · `skill_1`=Impacto · `skill_2`=Guarda · `skill_3`=Fenda · `skill_4`=reservada |
| D3 | Dano e atributos | Adotar a fórmula determinística do GDD 01 §1.3 e os atributos do Bastião; o dano fixo do protótipo é aposentado |
| D4 | Valores numéricos | Tabela única `EXPERIMENTAL` em `experimental-values.md`, derivada do GDD |
| D5 | Recursos | MP universal; **sem vigor/fúria** no slice; regeneração de MP conforme GDD 05 |
| D6 | Plugins | **Não** adotar Dialogic nem Maaack Game Template no slice |
| D7 | Inimigos e boss | IDs técnicos fixos; Garras de Musgo = categoria **sub-boss** |
| D8 | Equipamento | Slots `main_hand` e `chest`, templates fixos sem afixos aleatórios |

---

## D1 — Checkpoint do Bosque

**Decisão:** o único checkpoint do slice é `citadel`. `forest_01` **não** é checkpoint. Morrer no Bosque devolve o jogador à Cidadela (HP/MP restaurados pela cura do hub).

**Motivo:** GDD 05 §2.1 já define que a morte "normal retorna ao hub". Evita ampliar `WorldSaveSchema.ZONES`, bumpar `CONTENT_VERSION` e migrar por um benefício que o design não pede.

**Consequências:**
- `progress.checkpoint` permanece `"citadel"` durante todo o slice.
- Carregar um save sempre reaparece na Cidadela. "Preservar a zona" **não** é critério; preservar **decisões, inventário, XP e progresso** é.
- Progresso dentro do Bosque (boss derrotado, recompensa concedida) persiste por flags/IDs, não por checkpoint:
  - `world_flags["forest_01.moss_claws.defeated"] = true` (ID válido pelo regex do schema)
  - `granted_rewards` (T06) com `reward.forest_01.moss_claws.first_kill`
- `ZONES` só muda num cartão futuro de mapas 2–12, com migração.

## D2 — Mapa de slots de habilidade

| Ação (InputMap) | Teclas | Habilidade (GDD 01 §5) | MP | Estado no slice |
|---|---|---|---:|---|
| `attack_basic` | 1 | A1 — Golpe de Machado | 0 | existe; recebe fórmula (T01A) |
| `skill_1` | 2 / Q | A3 — Impacto do Baluarte | 12 | existe como "pesado técnico"; reconciliado (T01A/T01) |
| `skill_2` | 3 / E | A2 — Guarda do Crepúsculo | 8 | implementar (T01) |
| `skill_3` | 4 / R | A4 — Fenda de Aço | 20 | implementar (T01) |
| `skill_4` | 5 / F | — | — | **reservada**: avisa indisponibilidade, sem gasto |

**Motivo:** mantém `skill_1` como o que já roda (menor ruptura, testes e README coerentes) e distribui o restante na ordem do GDD. O Bastião tem 1 básico + 3 ativas; não existe quarta ativa no GDD.

**Emenda ao escopo (ver §9):** o critério "quatro slots com funções implementadas" passa a ser "ataque básico + três ativas implementadas e testadas; `skill_4` reservada". Passivas P1 (Postura Firme) e P2 (Peso do Juramento) ficam fora do slice.

## D3 — Dano e atributos: adotar a fórmula do GDD

**Decisão:** substituir o dano fixo do protótipo (12 básico / 24 pesado) pela fórmula determinística de GDD 01 §1.3, com inteiros e `floor`:

1. `base = P + W + 2×A + 2×(L−1)`
2. `ofensivo = floor(base × (1 + bonus_dano))` (bônus 0–100%)
3. `critico = floor(ofensivo × mult)` se crítico (um sorteio por acerto; RNG injetável)
4. `mitigado = floor(critico × 100 / (100 + defesa))` (DEF físico, MDEF arcano)
5. `resistido = floor(mitigado × (100 − resistencia)/100)`
6. `final = floor(resistido × (1 − reducao_temporaria))`, redução total ≤ 60%, mínimo 1 se acertou sem invulnerabilidade

Atributos do Bastião no nível 1: STR 12, DEX 6, INT 3, FTH 5, VIT 14 → **HP 172, MP 52** (`60+8×VIT`; `30+4×INT+2×FTH`). DEF = `armadura + floor(VIT/2) + 2×(L−1)`. Crítico: `min(25%, 5% + 0,25%×DEX)`; multiplicador `1,5`.

**Reconciliação do pesado:** o "pesado técnico" vira **Impacto do Baluarte**: custo 12 MP (já coincide), **P = 18**, interrupção de 0,5 s em inimigos comuns, boss não sofre stun. Os tempos 350/80/370 ms ficam como **EXPERIMENTAL**. Os 24 de dano fixo são aposentados.

**Consequência para os testes (T01A):** os testes que assumem HP 100 / MP 40 / dano 12 e 24 / HP do inimigo 64 devem ser atualizados e **listados no PR**. Comportamentos (fases, replay, custos, paredes, morte) não mudam.

## D4 — Valores numéricos

Todos os valores ainda não definidos pelos documentos estão em [experimental-values.md](experimental-values.md), numa tabela única, marcados `EXPERIMENTAL`. Regras:
- Código lê esses valores de `Resource`s em `content/`; nunca literais espalhados.
- Mudar um valor exige alterar `experimental-values.md` e o `.tres` no mesmo PR.
- Nenhum PR pode chamar estes valores de "balanceados".

## D5 — Recursos: MP, regeneração, sem vigor

- **MP** é o recurso das ativas (Guarda 8, Impacto 12, Fenda 20). Básico é gratuito. Stamina paga só o dash (máx. 100, custo 25).
- **Vigor/Fúria: fora do slice.** A barra auxiliar âmbar do GDD 05 não é implementada; `production-decisions.md` §2 já removeu a quarta barra e tornou fúria um buff passivo futuro.
- **Regeneração de MP** (GDD 05 §2.1): `1 + floor(INT/10)` MP/s (Bastião: 1 MP/s), **somente fora de combate**, após 3 s sem gastar MP e sem interação hostil. Fora de combate = 5 s sem atacar/receber ataque, sem alvo hostil engajado, sem DoT. Acumulador de tempo de simulação, independente de FPS.
- **HP:** sem regeneração natural; cura no hub restaura HP/MP; Poção de Vida pequena restaura até 50 HP; Frasco de Éter pequeno até 30 MP; cooldown compartilhado de 5 s; não consome com recurso cheio.

## D6 — Plugins externos

**Decisão:** **não adotar** Dialogic nem Maaack Game Template no slice.

**Motivo:** o save JSON próprio é a autoridade (o Maaack usa save `Resource`, que o próprio autor alerta poder executar script); Dialogic exige Godot ≥ 4.5 e um adaptador que ainda não tem utilidade com ~5 NPCs; ambos exigiriam versão fixa e auditoria de licença. Caixa de diálogo e menus simples próprios bastam para o loop.

**Reavaliar** após o primeiro playtest (T12), com versão fixa e licença registrada em `versions.md`.

## D7 — IDs técnicos de inimigos e categoria do boss

| Nome no jogo | `enemy_id` | Categoria (loot/XP) |
|---|---|---|
| Lobo Etéreo | `ethereal_wolf` | Comum |
| Cultista Cego-Guiado | `blind_cultist` | Comum |
| Garras de Musgo | `moss_claws` | **Sub-boss** (o primeiro "Boss de ato" é Silvano, mapa 3) |

IDs de instância de spawn: `<zone_id>.<enemy_id>.<nn>` (ex.: `forest_01.ethereal_wolf.01`). Esse ID é a chave de idempotência de loot/XP (T07), nunca o `enemy_id`.

Consequência: Garras de Musgo usa a linha **Sub-boss** das tabelas de GDD 03 (equipamento 100%, Raro 75% / Épico 25%, ouro 25–50, XP ×8). Não há fragmento nem receita neste mapa.

## D8 — Equipamento no slice

- Slots implementados: `main_hand` e `chest`. Nenhum outro slot.
- Itens de equipamento são **templates fixos sem afixos aleatórios** (sistema de afixos fora do slice). Existem 4 templates por slot (Comum, Mágico, Raro, Épico) para que a rolagem de raridade do GDD 03 §3.2 funcione sem afixos.
- Arma inicial: `training_axe` (W = 8, exemplo do GDD 01). Armadura inicial: `worn_tunic` (armadura 2).
- O drop respeita as tabelas do GDD 03; o item concreto é sorteado entre templates da raridade sorteada (injetando RNG).
- Estatísticas de equipamento alimentam a fórmula de D3 (`W`, `armadura`); trocar equipamento recalcula derivados e **não cura** (clamp ao novo máximo).

---

## 9. Emendas a documentos anteriores

| Documento | Trecho | Emenda |
|---|---|---|
| `project-scope.md` §3 (Personagem) | "quatro slots de habilidade" | Básico + três ativas (D2); `skill_4` reservada |
| `project-scope.md` §3 (Persistência) | "morte/checkpoint" | Checkpoint único = Cidadela (D1) |
| `project-scope.md` §4 | `2/Q` "pesado experimental" | `2/Q` = Impacto do Baluarte (D2/D3) |
| `offline-combat.md` | 12/24 de dano fixo; pesado 24 | Aposentados pela fórmula (D3); tempos mantidos como EXPERIMENTAL |
| `GDD/01` §5 | Ativas "usam vigor" | Ativas usam MP (já decidido em `production-decisions.md` §2) |
| `GDD/05` §2.1 | Barra de vigor | Fora do slice (D5) |
| `project-structure.md` §3 | `ZONES` sem `forest_01` | Não alterar; checkpoint só `citadel` (D1) |

## 10. Ainda em aberto (não bloqueiam o slice)

1. Criptografia local do save (origem da chave, recuperação, compatibilidade Windows).
2. Passivas P1/P2 e o buff passivo de fúria do Bastião.
3. Balanceamento real: tudo em `experimental-values.md` precisa de playtest.
4. Árvore de talentos e distribuição dos 3 pontos por nível (o slice só **acumula** pontos não gastos).
5. Colisão contínua de golpes rápidos e simulação da janela de esquiva do boss.
6. Licença do projeto e proveniência dos assets futuros.
