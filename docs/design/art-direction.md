# Direção de arte histórica — Godot 2D / 16-bit

> **Unity 3D:** seguir [direção vigente](unity-art-direction.md). Este documento preserva o briefing do Godot; pixels, filtro nearest, tiles e quadros de sprite não são requisitos de modelagem, UI ou animação do novo cliente.

> **Escopo histórico:** esta especificação de pixels, tiles e sprites rege o cliente Godot atual. O novo cliente 3D usa a paleta, as silhuetas e a atmosfera como referência, seguindo [Transição Unity](../engineering/unity-transition.md) e [Primeira rota 3D](first-route-3d.md). Dimensões 16×16/32×32 não são escala de modelagem 3D.

## Perspectiva e escala

Direção aprovada: top-down 3/4 com chão ortogonal e personagens mostrando frente e lateral. A impressão de profundidade vem da arte e da ordenação pelo ponto dos pés; não exige geometria isométrica. Combate continua em tempo real.

Tiles de 16×16 px, módulos de cenário de 32×32 px. Personagens jogáveis em células de 32×32 px, conforme a [Sprint 1](../GDD/01_classes_e_atributos.md); criaturas normais podem usar 32×32 ou 48×48 conforme silhueta. Bosses em 96×96 ou 128×128, compostos quando necessário. Colisor e hitbox são independentes da célula e nunca incluem cajados, capas ou efeitos decorativos.

Viewport de referência: 640×360. Preferir escala inteira com filtro nearest; em telas menores, adaptar UI e área visível sem cortar controles. A UI pode ser renderizada em resolução própria para texto ampliado. Não reduzir legibilidade para preservar uma grade estética.

## Paletas de referência

| Região | Base | Sombra | Destaque | Éter / perigo |
|---|---|---|---|---|
| Cidadela | Pedra #555866 | #202331 | Marfim #DDD5BD | Cristal #91D5DE |
| Ato I | Musgo #526044, marrom #604838 | #242A31 | #A1B17A | Violeta #A17AC4 / ácido #B5D95C |
| Ato II | Ocre #B59050, areia #8C8277 | #3B323D | #D8C7A3 | Sangue #A94750 / fogo #73BDE0 |
| Ato III | Chumbo #565E65 | #1F252A | Forja #DF8846 | Tóxico #A5D95D / bruma #A17AC4 |
| Ato IV | Noite #29334E | #141520 | Ouro #B8A16B | Estelar #AB88D0 |

Paletas são propostas de produção a validar em cenas. Alertas de combate combinam cor, forma, animação e som. Névoa e brilho nunca ocultam hitboxes ou antecipações de ataques.

## Interface

Painéis azul-escuros (#17213B), gradiente discreto, borda dupla de prata/marfim e texto claro com sombra de 1 px. Personalização da cor preserva contraste. Referências clássicas orientam composição; caixas, retratos, fontes, sons e sprites devem ser originais ou licenciados.

Retratos 64×64: neutro, preocupação, raiva e tristeza. Texto localizado em pt-BR, fonte pixel legível e fallback, tamanho ampliável, avanço manual e opção de mostrar tudo imediatamente. Foco visível, navegação por teclado e ícone além da cor para raridade. Diálogos pausam a simulação no modo offline. Menu operacional não força duas opções.

## Contrato de animação

| Estado | Frames por direção | Ritmo inicial |
|---|---|---|
| Idle | 4 | 6 fps |
| Caminhada | 6 | 10 fps |
| Ataque básico | 6 | 12 fps; duração real vem do domínio |
| Defesa / conjuração | 4 | 8 fps ou pose sustentada |
| Esquiva | 4 | 16 fps |
| Dano recebido | 2 | 10 fps |
| Morte | 6 | 8 fps, sem loop |

Quatro direções no primeiro asset; movimento pode ter oito direções. Espelhar apenas arte simétrica. Não espelhar armas assimétricas sem verificar sua posição. Origem nos pés consistente; metadados registram direção, frame, duração e estado. Eventos de dano pertencem à simulação, não ao número do frame desenhado.

## VFX, ambiente e áudio

Fogo, gelo e trovão usam animação quadro a quadro, com 6–10 frames por efeito inicial. Limitar partículas e luzes com perfil de desempenho. Névoa violeta no Bosque; bolhas verdes e reflexos roxos no Pântano; raízes rúnicas no Portal. Parallax somente decorativo, sem alterar leitura do chão.

Flashes críticos e tremor de câmera são opcionais, ajustáveis e desativados no modo de efeitos reduzidos. Silvano recebe tema original de timbres 16-bit, órgão e baixo pulsante; não reutilizar melodias das referências.

## Silvano

É o espírito guardião da floresta, manifestado na armadura de um antigo cavaleiro: prata envelhecida, cajado-espada, raízes e aura azul-escura. Assim, a aparência nova preserva sua identidade narrativa anterior.

## Primeiro pacote de assets

Cidadela e Bosque com tiles temporários; Bastião 32×32 em quatro direções; lobo e cultista; Garras de Musgo 128×128 (revisão do protótipo); retratos de Maera e Irian; UI, telegraph de investida, impacto e loot. Registrar autor, licença e arquivo-fonte de cada asset antes de integrar. Este documento não entrega sprites prontos.

## Proposta consolidada

Veja [proposta visual 01](visual-proposal.md) para composição, paleta, HUD com controles aprovados e briefing da imagem conceitual. Arte conceitual não é captura da build nem spritesheet pronto.
