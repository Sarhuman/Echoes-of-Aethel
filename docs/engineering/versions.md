# Versões e compatibilidade

> Atualização de 13/10/2026: primeira fatia jogável em `0.2.0-dev`. As linhas históricas abaixo sobre auditoria da `main` descrevem o marco anterior.

Revisão: 13/10/2026. Esta tabela registra o que existe no repositório; não anuncia lançamento comercial ou compatibilidade não testada.

| Camada | Versão / fonte | Política |
|---|---|---|
| Projeto | `0.2.0-dev` em `/VERSION` e `client/godot/project.godot` → `config/version` | Primeira fatia jogável; ambos devem mudar juntos |
| Engine validada | Godot `4.6.stable.official.89cea1439` | Manter 4.6 stable para reproduzir os testes; atualizar somente com validação |
| Engine Unreal instalada localmente | Unreal `5.8.3` (`UE_5.8/Engine/Build/Build.version`) | `client/unreal/EchoesOfAethel.uproject` associa 5.8; `Engine/Source` e MSVC não encontrados nesta inspeção, então compilação e abertura ainda pendem de validação |
| Formato do projeto Godot | `config_version=5` | Formato do arquivo, não versão do jogo |
| Schema do save | `SaveSystem.SCHEMA_VERSION = 1` | Não aumentar sem migração e testes de recuperação |
| Conteúdo do save | `WorldSaveSchema.CONTENT_VERSION = "world-save-2"` | Migrável de `world-save-1`; separado da versão do envelope |
| Renderer | `gl_compatibility` | Não alterado pela galeria ou arte conceitual |
| Maaack / Dialogic / Tactical RPG | Sem instalação no projeto | Referências de arquitetura/produção; não são dependências de runtime |
| Python/FastAPI/PostgreSQL | Planejados para etapa online | Sem implementação, lockfile ou versão de runtime para atualizar aqui |
| Releases / tags | Nenhuma publicada até este trabalho | Export local Windows e artefato CI; versão dev não é release pública |

## Resultado da auditoria

A main estava no commit inicial. Os PRs #1–#18 formavam uma cadeia linear e foram integrados em ordem, com preservação de histórico. A árvore consolidada corresponde à base verificada. A galeria e o versionamento entram em entrega própria.

Não havia versão de aplicação. Adotado `0.1.0-dev` para identificar o protótipo, sem criar release fictícia ou afirmar export Windows. O nome da aplicação e o caminho de userdata foram preservados, para não mudar a localização dos saves por efeito colateral.

A engine 4.6 foi mantida; este trabalho não pretende descobrir ou instalar a versão mais recente do Godot. O schema permanece v1; propostas de v2 em anexos não foram confundidas com código integrado. Reconciliar formatos a partir dos arquivos reais antes de qualquer migração.

## Referências avaliadas

READMEs consultados durante esta sessão: Maaack declara compatibilidade 4.4+; Dialogic exige 4.5+; Tactical RPG descreve base 4.3. Esses números não são dependências instaladas, nem prova de integração no Godot 4.6. Quando houver adoção: selecionar release/commit, registrar licença e testar adaptadores antes de atualizar.

## Gates para uma release

- Versão de código, changelog e artefato exportado concordam.
- Suítes headless e teste de save/reload aprovados na engine fixada.
- Export Windows abre fora do editor, com instruções de controles.
- Mapa/quest/recompensa/checkpoint do marco anunciado realmente concluíveis.
- Assets com proveniência; política de licença definida pelo proprietário.
- CI e processo de empacotamento reproduzíveis.

CI, export Windows e primeira jornada jogável foram implementados nesta branch. A arena de três encontros integrada à `main` como PR #20 foi preservada em `res://playtest/arena.tscn`; o projeto abre `res://scenes/game_root.tscn`. São 16 suítes e 308 verificações locais. Playtest humano e política de licença permanecem antes de uma release pública.
