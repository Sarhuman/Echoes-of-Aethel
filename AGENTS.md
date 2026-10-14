# AGENTS.md — Guia para o GPT desenvolver Echoes of Aethel

Leia este arquivo inteiro antes de criar ou alterar qualquer arquivo. Ele vale para toda a árvore.

## 1. Projeto em uma frase

ARPG 2D offline, dark fantasy, pixel art, top-down 3/4 com projeção 2:1. **Godot 4.6 stable, GDScript, renderer GL Compatibility.** Plataforma inicial: Windows. Raiz do projeto Godot: `client/godot/` (`res://` aponta para essa pasta).

Estado: primeira fatia jogável `0.2.0-dev`, com Cidadela, Bosque, combate, inimigos, boss, inventário, loot, quest, diálogos, menus, save v2 e arte original de runtime. A referência de validação atual é o README e o workflow de CI; os cartões T00–T12 registram a implementação, mas parte de seus textos foi escrita quando estes módulos ainda eram apenas planejados.

## 2. Fontes de verdade (ordem de precedência)

1. `docs/engineering/production-decisions.md` — decisões aprovadas pelo Game Director; prevalece sobre o resto.
2. `docs/engineering/decisions-first-slice.md` — decisões D1–D8 do slice e emendas ao escopo (seção 9). **Não reabra** pontos já resolvidos ali.
3. `docs/engineering/project-scope.md` — escopo da primeira fatia e controles aprovados (com as emendas acima).
4. `docs/engineering/experimental-values.md` — **todos** os números do slice; nunca coloque literal de balanceamento no código.
5. `docs/engineering/project-structure.md` — mapa de pastas, camadas e contratos.
6. `docs/engineering/tasks/` — cartões de tarefa ordenados; trabalhe **um cartão por vez**.
7. `docs/GDD/` e `docs/design/` — design detalhado. Documento não prova que o código existe.

Se duas fontes divergirem, siga a de menor número e registre a divergência no PR. **Nunca invente** números de balanceamento, IDs narrativos ou nomes de personagens: use os que estão em `experimental-values.md` e nos docs; se faltar algum valor, adicione-o àquela tabela marcado `NOVO`/`EXPERIMENTAL` no mesmo PR.

## 3. Regras de ouro

1. **Não altere código existente sem necessidade do cartão.** Estender é melhor que reescrever. Os arquivos em `core/`, `combat/`, `entities/` têm testes; se mudar comportamento, atualize os testes no mesmo PR.
2. **Não renomeie nem mova** arquivos existentes (`res://combat/basic.tres` é carregado por `preload`). Se precisar mover, atualize todas as referências e rode todos os testes.
3. **Não escreva arquivos `.uid`.** O editor Godot os gera; commite os que ele criar.
4. **Não crie `.import`, `.godot/` nem nada de cache.** Já estão no `.gitignore`.
5. **Todo código novo precisa de teste headless** em `client/godot/tests/` (padrão da seção 6).
6. **Sem addons.** Dialogic e Maaack Game Template **não** serão usados no slice (D6). Qualquer outro plugin exige decisão explícita, versão fixa e licença registrada em `docs/engineering/versions.md`.
7. **Sem assets de terceiros** sem origem e licença registradas. Para arte original gerada para este projeto, documente a proveniência em `docs/media/runtime-art.md`; preserve a distinção entre conceitos e capturas reais.
8. **Saves nunca carregam `Resource` nem scripts.** Somente JSON validado (`WorldSaveSchema`).
9. **Cuidado com a Cidadela:** nenhum dano, nenhum inimigo ativo, serviços essenciais sempre disponíveis.

## 4. Convenções de GDScript (Godot 4.6)

- **Indentação com TAB** (padrão dos arquivos existentes). Nunca misture com espaços.
- Tipagem estática sempre que possível: `var hp: int = 0`, `func f(x: float) -> bool:`.
- Arquivos em `snake_case.gd`; `class_name` em `PascalCase`, único no projeto. Autoloads **não** usam `class_name` igual ao nome do autoload.
- IDs de domínio (`actor_id`, `quest_id`, `item_id`, `zone_id`) são strings estáveis `^[a-zA-Z0-9_.:-]+$`, até 128 chars (`WorldSaveSchema.valid_id`). Nunca derive ID de nome de nó.
- `EventBus` notifica **fatos já confirmados**. Input/UI enviam **comandos** a serviços; não emita consequência no bus para outro listener executar.
- Domínio (`combat/`, `inventory/`, `quest/`, `services/`) **não referencia** `Node` de cena, UI nem áudio. Prefira `RefCounted`/`Resource` e funções estáticas puras.
- Apresentação (HUD, animação, áudio, VFX) **nunca** concede dano, loot, XP ou progresso.
- Tempo de simulação via `delta` de `_physics_process`; nunca contagem de frames. RNG injetável (`RandomNumberGenerator` passado por parâmetro) para testes determinísticos.
- Valide entradas com `is_finite()`, intervalos e IDs. Comando recusado **não altera estado nem recursos**.
- Comandos idempotentes: repetir um comando concluído devolve o resultado existente, sem duplicar efeito, loot ou XP.
- Mensagens ao jogador em **português**; identificadores de código em **inglês**.
- Parâmetros de conteúdo em `Resource` tipado dentro de `content/` (conteúdo confiável do projeto, não do save).

## 5. Como entregar um cartão

1. Crie uma branch `feat/<id-do-cartao>-<slug>` a partir da `main` atualizada.
2. Leia o cartão em `docs/engineering/tasks/` e os docs que ele cita.
3. Crie/edite **somente** os arquivos listados no cartão (mais testes e docs correspondentes).
4. Rode a verificação completa: `bash tools/run_tests.sh`. Todas as suítes devem passar.
5. Atualize `CHANGELOG.md` e a tabela de estado em `docs/engineering/project-structure.md` (coluna Estado).
6. Abra PR descrevendo: o que foi entregue, o que ficou pendente, como validou (comandos e resultado real). **Não declare** testes, balanceamento ou export como aprovados sem tê-los executado.
7. Não faça merge sozinho.

## 6. Padrão de teste headless

Cada suíte é um script `extends SceneTree` em `client/godot/tests/`, nomeado `test_*.gd`, com `check(bool, label)` e saída com código ≠ 0 em falha. Testes que dependem de cena podem ficar em `tests/integration/`. `tests/reload_probe.gd` é sonda auxiliar de save e **não** é suíte. Copie o padrão de `tests/test_combat.gd` (não há helper compartilhado; se criar um, faça-o em cartão próprio):

```gdscript
extends SceneTree
var checks := 0
var failures := 0
func _initialize() -> void: call_deferred("run")
func check(value: bool, label: String) -> void:
	checks += 1
	if not value:
		failures += 1
		push_error(label)
func run() -> void:
	# ... verificações ...
	print("%d checks, %d failures" % [checks, failures])
	quit(1 if failures > 0 else 0)
```

Execução de uma suíte: `godot --headless --path client/godot --script res://tests/<arquivo>.gd`.
Importação/parse de todos os scripts: `godot --headless --editor --path client/godot --quit` (deve terminar sem `SCRIPT ERROR`).

Suítes que gravam save usam e apagam dados do harness: **nunca em paralelo** sobre o mesmo slot.

## 7. Controles aprovados (não alterar sem autorização)

Mover: WASD/setas · Básico: `1` · Habilidade 1: `2`/`Q` · Hab. 2: `3`/`E` · Hab. 3: `4`/`R` · Hab. 4: `5`/`F` · Dash: Espaço · F6 reinicia cena técnica · F8 alterna apresentação · F9 reduz efeitos. Ações no InputMap: `move_*`, `dash`, `attack_basic`, `skill_1..4`, `debug_restart`. W é exclusivo de movimento.

## 8. Fora do escopo da primeira fatia (não implemente)

Multiplayer/PvP, backend/FastAPI, contas, nuvem, telemetria, outras quatro classes, atos II–IV, mapas 2–12, geração procedural, crafting avançado, monetização, endgame, modo Eclipse, versões web/mobile.

## 9. O que fazer em caso de dúvida

Pare e registre a pergunta no PR (seção "Decisões pendentes"). Prefira entregar menos, testado, a entregar mais, suposto.
