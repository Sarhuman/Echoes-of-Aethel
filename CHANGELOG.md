# Changelog

## 0.2.0-dev — 2026-10-13

- Arena como cena inicial: três encontros, HUD, pausa, vitória/derrota e reinício.
- Lobo e Cultista experimentais com parâmetros e antecipações diferentes.
- Navegação A* compartilhada para obstáculos estáticos e memória da última posição visível por três segundos.
- Recuperação de 30 HP, MP e stamina entre encontros; sem gravar progressão.
- Cena técnica preservada; F8/F9 permanecem exclusivos dela.
- 144 verificações headless: 125 regressões + 19 de arena, incluindo desvio físico, pausa e transições.
- Execução pelo Godot 4.6; sem export Windows ou avaliação gráfica neste ambiente.


## 0.1.0-dev — 2026-10-13

Primeiro identificador de desenvolvimento do protótipo técnico; sem tag, release binária ou campanha completa.

### Integrado

- PRs #1–#11: documentação de design, campanha, classes, arte e arquitetura.
- PR #12: save de mundo JSON, integridade e recuperação.
- PRs #13–#16: ator/FSM, escopo de produção, Player/dash e apresentação opcional.
- PR #17: ataques offline reais, custos e registro de alvos.
- PR #18: controles 1–5/Q/E/R/F, escopo consolidado e proposta visual.
- Cadeia #1–#18 integrada à main em ordem de dependência, mantendo commits.

### Apresentação e manutenção

- README com três imagens conceituais, estado implementado, pendências, execução e controles.
- PNGs originais e manifesto de proveniência/integridade da galeria.
- `VERSION` e `config/version` sincronizados em `0.1.0-dev`.
- Versões do Godot, formato de projeto e save documentadas separadamente.

### Validação e limites

125 verificações headless aprovadas em Godot 4.6 Linux: save 27, FSM 25, Player 19, apresentação 13 e combate 41. A atualização de metadados de versão recebe importação/smoke adicionais. Sem CI automatizada, benchmark, playtest visual ou export Windows. Imagens são conceitos gerados por IA, não screenshots de gameplay.
