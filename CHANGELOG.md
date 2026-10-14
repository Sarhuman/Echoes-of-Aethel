# Changelog

## 0.2.0-dev — 2026-10-13

- Ajustes de 14/10 após novo playtest: Cultista Cego e Garras de Musgo recebem quatro quadros de locomoção com pés alinhados e sombras de contato; movimento deixa de aplicar oscilação vertical. Inimigos passam a colidir com cenário, sem prender o Bastião por contato físico, preservando ataques por proximidade. Validação: 19 suítes / 385 verificações, export Windows e inicialização por 120 quadros.

- Ajustes visuais de 14/10: recorte dos atlas usa células de 627 px e os quadros do Bastião alinham os pés; indicadores de ataque inimigo recebem borda clara e preenchimento progressivo; marcadores cinzentos de pilares saem do Bosque. Validação: 19 suítes / 377 verificações, export Windows e inicialização por 120 quadros.

- Ajustes de 14/10 após playtest: cinco NPCs passam a ser desenhados na Cidadela; Lobo Etéreo tem quatro quadros de locomoção e contorna o poço; o jogador deixa de ser preso pela colisão de inimigos. Impacto considera o corpo de Garras de Musgo, com números de dano e “MISS”; Guarda exibe um escudo mais legível. As outras quatro classes seguem documentadas para expansão, sem seleção jogável nesta versão. Validação: 19 suítes / 361 verificações, export Windows e inicialização do executável por 120 quadros.

- Atualização de 14/10: inimigos derrotados deixam de gerar marcador residual; separação física, esquiva que atravessa atores e barreira acompanhando a borda do abismo no Bosque. Comércio ampliado com equipamento, antídoto e venda idempotente de materiais/peças. Três composições e seis efeitos originais gerados por código, integrados às opções de som. Validação: 18 suítes / 338 verificações, export Windows e inicialização do executável por 120 quadros.

- Atualização de 14/10: medidores de Vida, Éter e Fôlego com molduras e valores; compras no hub com Lyra e Sera (10/12 ouro), débito/entrega atômicos e proteção contra repetição. Capturas de QA agora exigem slot de teste isolado. A arena técnica da `main` foi preservada como playtest opcional. Validação ampliada para 16 suítes / 308 verificações.

- Primeira jornada jogável: Cidadela, Bosque, três inimigos, Garras de Musgo, quest com duas escolhas, diálogo, consequência no hub e retorno seguro.
- Fórmula de dano e atributos do Bastião, guarda, três habilidades, regeneração, inventário de 60 posições, equipamento, consumíveis, loot e XP.
- Save de conteúdo v2 com migração de v1, backup, recompensas excedentes recuperáveis e testes de recarga.
- Mapas, sprites e ícones originais em arte pintada de fantasia sombria, com grade de itens e capturas reais do Godot no README. HP, MP e stamina receberam a paleta vermelho, azul e amarelo.
- Animações de quatro quadros para caminhada e machado, efeitos visíveis de golpe, Fenda, Guarda e esquiva. Clique esquerdo/direito como aliases de ataque/Impacto; aviso explica o bloqueio de combate na Cidadela.
- Menu inicial, pausa, controles, opções, diário, HUD e sons simples sintetizados.
- Export Windows gerado e inicializado por 120 quadros no smoke. CI configurado para testes e export.
- Playtest humano, animação, áudio final, balanceamento e campanha completa permanecem pendentes.

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
