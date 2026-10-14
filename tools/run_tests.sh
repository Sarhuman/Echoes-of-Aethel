#!/usr/bin/env bash
# Executa a importação e todas as suítes headless (tests/test_*.gd e tests/integration/test_*.gd).
# Uso (da raiz do repositório):  bash tools/run_tests.sh
# Variável opcional: GODOT=/caminho/para/godot
# Suítes rodam em sequência: algumas usam o mesmo slot de save do harness.
set -euo pipefail
GODOT="${GODOT:-godot}"
PROJECT="client/godot"

if ! command -v "$GODOT" >/dev/null 2>&1; then
	echo "Godot não encontrado ('$GODOT'). Instale o Godot 4.6 stable ou defina GODOT=..." >&2
	exit 2
fi

echo "== Godot: $("$GODOT" --version)"
echo "== Importação / parse de scripts"
if ! import_log="$("$GODOT" --headless --editor --path "$PROJECT" --quit 2>&1)"; then
	printf '%s\n' "$import_log" >&2
	exit 1
fi
if [[ "$import_log" == *"SCRIPT ERROR"* || "$import_log" == *"Parse Error"* ]]; then
	printf '%s\n' "$import_log" >&2
	echo "FALHA: erros de script na importação." >&2
	exit 1
fi

failed=0
total=0
shopt -s globstar nullglob
suites=("$PROJECT"/tests/**/test_*.gd)
if [ "${#suites[@]}" -eq 0 ]; then
	echo "FALHA: nenhuma suíte encontrada." >&2
	exit 1
fi
for suite in "${suites[@]}"; do
	rel="res://${suite#"$PROJECT"/}"
	total=$((total + 1))
	echo "== $rel"
	if ! suite_log="$("$GODOT" --headless --path "$PROJECT" --script "$rel" -- --test-harness 2>&1)"; then
		echo "FALHA: $rel" >&2
		failed=$((failed + 1))
	elif [[ "$suite_log" == *"SCRIPT ERROR"* || "$suite_log" == *"Parse Error"* ]]; then
		failed=$((failed + 1))
	fi
	printf '%s\n' "$suite_log"
done

echo "== Suítes: $total; com falha: $failed"
[ "$failed" -eq 0 ]
