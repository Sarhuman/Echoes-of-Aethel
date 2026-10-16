# Changelog

## Não lançado — revisão de identidade visual (16/10/2026)

- Revisão solicitada das pranchas de heróis/antagonistas, com achados confirmados em seis imagens, prioridades e escopo restante explícito.
- Galeria marcada em revisão; critérios de silhueta, câmera, rostos, materiais e coerência de turnaround registrados antes de aprovação para modelagem final.
- Originais e proveniências preservados; nenhum PNG, variante, modelo ou asset substituído nesta etapa.

## Não lançado — formação regional e leitura de ameaça (16/10/2026)

- Faixas experimentais de nível dos monstros nos 12 mapas e duas dungeons, independentes do nível do herói e distintas de nível de item; hubs seguros preservados.
- HUD planejado com nome/nível/cor, símbolo de ameaça e identificação separada de boss/elite; acesso a especialização sem depender de popup.
- Responsáveis da Cidadela atribuídos às cinco classes; regiões seguintes recebem pessoas locais distintas, sem clonagem narrativa.
- Baseline de especialização: níveis 5/10/15/20/25/30, quatro nós por direção e seis investimentos totais; primeira redistribuição gratuita, depois 5 vezes o nível em ouro.
- Dois modificadores iniciais do Bastião detalhados para teste futuro. Somente design; números não simulados e nenhuma alteração de combate ou UI ativa.

## Não lançado — especializações de classe (16/10/2026)

- Design incorporado para primeira escolha no nível 5, aviso discreto, consulta no menu e confirmação por mentor em hub; Bram para o Bastião.
- Seleção inicial separada dos pontos dos nós básicos; dois caminhos propostos por classe, sem alterar controles ou vincular build à moralidade.
- Fluxos de confirmação/adiamento, persistência, respec e critérios de aceite especificados. Efeitos, valores, demais mentores, UI e runtime continuam pendentes.

## Não lançado — núcleo C# de memória do mundo (16/10/2026)

- Núcleo isolado para escolhas e consequências das 12 quests, com projeção sem efeitos futuros, afinidades e marcos canônicos.
- Persistência JSON limitada/versionada, comparação de versão, escrita por substituição, backup e recusa de corrupção. Não migra saves anteriores.
- Catálogo gerado do GDD e harness C# com as 4.096 combinações de escolhas; compilação/testes executados em Mono 6.8, fora da Unity.
- CI dedicada e guia de integração transacional. Cliente Unity, quest completa, cenas, combate, loot e apresentação visual permanecem pendentes.

## Não lançado — revisão Unity e campanha completa (16/10/2026)

- Direção Unity documentada; clientes Godot/Unreal preservados e sem migração automática.
- Proposta editorial de interações dos 12 mapas, quatro hubs, três secundárias e duas dungeons.
- Novos nomes e encenações marcados como proposta, sem alterar IDs/opções/fragmentos canônicos.
- Títulos dos finais no resumo corrigidos conforme decisões de produção.
- JSON de design, documento gerado e validador com 13 testes; enumeração das 4.096 rotas de escolhas do modelo documental, sem afirmar teste de gameplay.
- Gates restantes explícitos: aprovação editorial, catálogo/economia, câmera/layout, assets, runtime Unity e build/playtest.


## 0.2.0-dev — 2026-10-13

- Verificação de 15/10: UnrealBuildTool 5.8.3 passou da validação do Windows SDK, mas bloqueou o build C++ por falta de MSVC x64/x86 válido (mínimo 14.38.33130, recomendado 14.50.35717). Nenhuma cena ou runtime Unreal foi declarado compilado.

- Revisão de 14/10: áudio recupera volume integral após narração, cinemática cancela deslocamento ativo, `Intelligence` evita colisão de nome com Windows, targets fixam defaults da UE 5.8. `.gitattributes` padroniza LF e prepara LFS para assets Unreal futuros; ordem de integração das três PRs registrada.

- Contrato Unreal de 14/10: fórmulas inteiras e validação dos cinco atributos em C++; especificação de save, recompensa única e retorno seguro de dungeon revisada contra os GDDs. Portal e loot de runtime aguardam transação persistente e compilação.

- Preparação Unreal de 14/10: diretor e gatilho de introdução de chefe, crossfade musical/narração e passos por material físico adicionados como infraestrutura C++. Ainda sem mapas, sons, animações ou build compilada; a fase de 50% permanece especificação para playtest futuro.

- Direção de produção de 14/10: documentada a transição para uma prova 3D em Unreal, com índice dos 12 mapas, ficha Cidadela → Bosque, interiores acessíveis propostos e duas fontes de loot fixo/idempotente. Os documentos Godot/16-bit foram marcados como históricos para a nova engine; nenhum `.uproject` ou asset 3D foi declarado pronto.

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

