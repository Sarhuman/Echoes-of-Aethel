# T12 — CI, estrutura final e export Windows

**Depende de:** T01–T11

## Objetivo

Automatizar `tools/run_tests.sh` em CI, mover recursos de ataque para `content/attacks/` com referências atualizadas, e gerar executável Windows para playtest.

## Arquivos a criar/alterar

- `.github/workflows/tests.yml` (Godot 4.6 headless, executa `tools/run_tests.sh`)
- `client/godot/export_presets.cfg`
- Mover `combat/basic.tres` e `combat/heavy.tres` → `content/attacks/` atualizando `player.gd` e testes
- Atualizar `README.md`, `CHANGELOG.md`, `docs/engineering/versions.md`

## Contrato

- Versão do Godot fixada igual à de `versions.md` (4.6 stable).
- Executável abre fora do editor e completa o loop sem console.
- `VERSION` e `config/version` mudam juntos.
- Registrar licenças de todos os assets.

## Critérios de aceite

- CI verde com todas as suítes.
- Playtest Windows: loop completo registrado em relatório (sem inventar resultados).

## Testes obrigatórios

Toda a suíte via `bash tools/run_tests.sh`; verificar que nenhuma referência `res://combat/*.tres` restou.

## Fora de escopo

Release pública, assinatura de código, outras plataformas.

## Leia antes

`docs/engineering/versions.md` (Gates para uma release); `docs/engineering/github-workflow.md`.
