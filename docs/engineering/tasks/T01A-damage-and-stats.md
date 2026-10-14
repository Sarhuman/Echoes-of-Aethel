# T01A — Fórmula de dano, atributos do Bastião e regeneração

**Depende de:** T00 · **Decisões:** D3, D4, D5 em [`decisions-first-slice.md`](../decisions-first-slice.md) · **Valores:** [`experimental-values.md`](../experimental-values.md) §1–2

## Objetivo

Trocar o dano fixo e os recursos fixos do protótipo (HP 100, MP 40, dano 12/24) pela fórmula determinística do GDD 01 §1.3 e pelos atributos reais do Bastião, mantendo todos os comportamentos já testados (fases, replay, custos, paredes, morte).

## Arquivos a criar/alterar

- `client/godot/combat/damage_calculator.gd` (`class_name DamageCalculator`, `RefCounted`, funções `static`)
- `client/godot/content/classes/class_definition.gd` (`class_name ClassDefinition`, `Resource`) e `content/classes/twilight_bastion.tres`
- `client/godot/combat/character_stats.gd` (`class_name CharacterStats`, `RefCounted`: atributos, nível, derivados)
- `client/godot/combat/resource_regen.gd` (`class_name ResourceRegen`, `RefCounted`: regeneração de MP por acumulador)
- **Alterar** `combat/attack_definition.gd`: adicionar `power: int` (P), `scale_attribute: StringName` (`&"STR"` por padrão) e `interrupt_seconds: float`; **manter** `damage` como legado até esta entrega terminar e então removê-lo, atualizando `signature()`.
- **Alterar** `combat/basic.tres` (P=10) e `combat/heavy.tres` (vira Impacto: P=18, 12 MP, 350/80/370 ms). **Não mover** os arquivos (ver T12).
- **Alterar** `entities/player/player.gd`: ler HP/MP/DEF de `CharacterStats` + `ClassDefinition`; chamar `DamageCalculator` ao resolver acerto; usar `ResourceRegen`.
- **Alterar** `entities/ai/enemy_fsm.gd` somente para que o dano ao jogador passe por `DamageCalculator.mitigate` (DEF/MDEF do alvo).
- `client/godot/tests/test_damage.gd` (novo); atualizar `tests/test_combat.gd`, `tests/test_player.gd`, `tests/test_enemy_fsm.gd`.

## Contrato

```gdscript
class_name DamageCalculator
extends RefCounted
# Passos 1-6 de GDD 01 §1.3. Inteiros e floor em etapas explícitas.
static func base_damage(power: int, weapon: int, attribute: int, level: int) -> int   # P + W + 2*A + 2*(L-1)
static func offensive(base: int, bonus: float) -> int                                  # bonus em [0,1]
static func critical(offensive_value: int, rng: RandomNumberGenerator, chance_bp: int, multiplier: float) -> Dictionary  # {value, crit}
static func mitigate(value: int, defense: int) -> int                                  # floor(v*100/(100+def))
static func final_damage(value: int, resistance_pct: int, temp_reduction: float) -> int  # res -50..75, red<=0.6, minimo 1
static func compute(spec: Dictionary, rng: RandomNumberGenerator) -> Dictionary        # {final, crit, nominal}
```

```gdscript
class_name CharacterStats
extends RefCounted
func hp_max() -> int   # 60 + 8*VIT + 6*(L-1)
func mp_max() -> int   # 30 + 4*INT + 2*FTH + 3*(L-1)
func defense(armor: int) -> int           # armor + floor(VIT/2) + 2*(L-1)
func magic_defense(ward: int) -> int      # ward + floor((INT+FTH)/3) + (L-1)
func crit_chance_bp() -> int              # min(2500, 500 + 25*DEX)
```

```gdscript
class_name ResourceRegen
extends RefCounted
# Acumulador de tempo de simulação, independente de FPS.
func tick(delta: float, mp: float, mp_max: float, in_combat: bool, spent_mp_recently: bool) -> float
func notify_combat_event() -> void   # atacar/receber ataque zera o relógio de 5 s
func notify_mp_spent() -> void       # zera o relógio de 3 s
```

- Regras: MP regenera `1 + floor(INT/10)` por segundo só fora de combate, 3 s após gastar MP e 5 s após evento de combate. Nunca regenera HP.
- Valores inválidos (`NaN`, `INF`, negativos fora do domínio) são recusados sem alterar estado.
- Cura (poção, hub) **não** é regeneração: é responsabilidade de T05/T03.
- Equipamento ainda não existe: use `W = 8` e `armor = 2` fixos vindos de `ClassDefinition.starter_*` até T05.

## Critérios de aceite

- Exemplo do GDD 01 reproduzido exatamente: STR 12, `W=8`, `P=10` → base 42; contra DEF 20 → 35; crítico → 63 → 52; com guarda 40% → 21.
- Bastião nível 1: HP 172, MP 52, DEF 9 (armadura 2), MDEF 2.
- Dano dos ataques contra `ethereal_wolf` (DEF 5): básico 40; Impacto 47; Fenda 59 (Fenda ainda não é jogável: valide pela calculadora).
- Regeneração não depende de FPS (mesmo resultado com passos de 16 ms e 100 ms).
- Todas as verificações existentes passam, exceto as que dependiam de HP 100 / MP 40 / dano 12 e 24 / HP do inimigo 64; **liste essas verificações no PR** e atualize os valores.

## Testes obrigatórios

Exemplo do GDD, piso em cada etapa, dano mínimo 1, crítico com RNG de seed fixa (5% e 100% em teste), resistência e redução fora dos limites, DEF/MDEF por tipo, HP/MP por nível 1 e 2, regeneração (bloqueio de 3 s e 5 s, acumulador, FPS diferente), entradas inválidas, `AttackDefinition.valid()` com os novos campos.

## Fora de escopo

Equipamento real (T05), habilidades novas (T01), afixos, bônus de dano por passivas, distribuição de pontos de atributo, Bruma/promoção de elites.

## Leia antes

`docs/GDD/01_classes_e_atributos.md` §1.1–1.3; `docs/GDD/05_combate_e_hud.md` §2.1; `docs/engineering/offline-combat.md`; `decisions-first-slice.md` D3/D5.
