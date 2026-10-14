# T00 — Catálogo de conteúdo validado

**Depende de:** nenhum (primeiro cartão)

## Objetivo

Criar o índice central de conteúdo (`Resource`s em `content/`) com validação de IDs únicos e referências existentes, para que os próximos cartões registrem conteúdo sem lógica espalhada.

## Arquivos a criar/alterar

- `client/godot/core/content_catalog.gd` (`class_name ContentCatalog`, `extends RefCounted`)
- `client/godot/tests/test_content_catalog.gd`
- Nenhuma alteração em arquivos existentes.

## Contrato

```gdscript
class_name ContentCatalog
extends RefCounted
# Carrega .tres de uma pasta de content/ e indexa por ID.
func load_directory(kind: StringName, dir_path: String, id_property: StringName) -> Array[String]  # retorna lista de erros (vazia = ok)
func get_by_id(kind: StringName, id: StringName) -> Resource  # null se ausente
func ids(kind: StringName) -> Array[StringName]
func validate_references(kind: StringName, ref_property: StringName, target_kind: StringName) -> Array[String]
```
- IDs seguem `WorldSaveSchema.valid_id`. ID duplicado = erro, nunca sobrescreve.
- Somente `Resource` de `res://content/`; recusa caminhos fora dessa raiz.
- Pasta vazia (`.gitkeep`) não é erro.

## Critérios de aceite

- IDs duplicados e inválidos são reportados.
- Referência inexistente é reportada com `kind`, ID e propriedade.
- Carregar `res://content/attacks` com os `.tres` copiados de teste funciona (use `Resource`s criados em memória nos testes; não dependa de arquivos finais).

## Testes obrigatórios

Duplicado, ID inválido (`""`, espaço, >128), referência quebrada, pasta vazia, caminho fora de `res://content/`.

## Fora de escopo

Cache em disco, hot reload, editor plugin.

## Leia antes

`AGENTS.md` §4; `docs/engineering/architecture.md` (Conteúdo); `production-decisions.md` §1.
