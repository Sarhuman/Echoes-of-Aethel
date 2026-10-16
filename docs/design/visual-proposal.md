# Echoes of Aethel — proposta visual 01

> Documento histórico da direção **Godot 2D**. Paleta, composição e leitura continuam referências; pixel art 16-bit, projeção 2:1 e medidas em pixels não são requisitos do cliente Unreal 3D. Ver [transição Unreal](../engineering/unreal-transition.md) e [decisões D01–D08](../engineering/diablo-direction-decisions.md).

13/10/2026 · direção de produção e briefing de imagem conceitual.

## Conceito: a luz que ainda resiste

Pedra antiga, ferro azul e raízes invadidas por uma bruma violeta. A Cidadela protege uma pequena chama humana; o Bosque parece reconhecer o herói. A imagem deve comunicar o contraste entre abrigo e ameaça antes de qualquer explicação em texto.

A proposta mantém pixel art 16-bit, vista superior em três quartos e combate legível. Trabalhar formas grandes, recortes de luz e materiais bem separados; evitar névoa uniforme, excesso de ornamentos e escuridão que torne o caminho invisível. O tom é melancólico e ameaçador, com sinais de vida no hub.

## 1. Câmera e construção da cena

Perspectiva top-down 3/4 com projeção simulada 2:1 e profundidade pelo ponto dos pés. O chão continua organizado de modo consistente com a física; não converter automaticamente todo o projeto em grade isométrica. Arte não determina hitbox.

A cena conceitual mostra a passagem entre o portão da Cidadela e o Bosque. O Bastião ocupa o terço central inferior, pequeno diante das ruínas, mas imediatamente identificável pelo escudo âmbar. Uma trilha livre guia o olhar até um lobo etéreo; uma silhueta maior de raízes ao fundo sugere Garras de Musgo. A ameaça maior não compete com a leitura do combate próximo.

O portão aquecido por lanternas ancora a esquerda; raízes, pedras e bruma conduzem à direita. Diferenciar caminhos caminháveis por textura e valor, obstáculos por volume e borda, perigos por contorno e animação. Árvores em primeiro plano só emolduram e precisam admitir transparência/ocultação no jogo quando encobrirem o personagem.

## 2. Paleta proposta

| Uso | Cor | Função |
|---|---|---|
| Fundo profundo | `#141820` | Separar planos, sem cobrir o caminho |
| UI azul-noite | `#17213B` | Painéis e caixas de diálogo |
| Pedra da Cidadela | `#555866` | Abrigo, construção humana e ruínas |
| Musgo | `#526044` | Bosque e superfícies naturais |
| Marfim | `#DDD5BD` | Texto, bordas e detalhes claros |
| Âmbar | `#D9A441` | Escudo, lanternas e pontos de apoio |
| Éter ciano | `#91D5DE` | Magia estável e pistas interativas |
| Bruma violeta | `#A17AC4` | Corrupção e atmosfera |
| Perigo ferrugem | `#C75A4C` | Sinais hostis, junto de forma/ritmo |

Não depender só de vermelho/verde. Telegraph possui borda, movimento e som; habilidade indisponível usa ícone/estado textual, não apenas cor apagada. Flash e shake devem poder ser desligados.

## 3. Personagens e silhuetas

| Elemento | Identidade visual | Produção inicial |
|---|---|---|
| Bastião | Armadura azul-ferro, pano gasto, escudo âmbar, machado de lâmina simples | Célula 32×32; quatro direções; origem nos pés |
| Lobo Etéreo | Corpo horizontal, costelas sugeridas por luz fria, olhos pontuais | Referência 48×32; ataque legível pela postura |
| Cultista Cego | Silhueta vertical estreita, faixa nos olhos, foco de magia violeta | Referência 32×48; conjuração com antecipação |
| Garras de Musgo | Massa de raízes e pedra, ombros assimétricos, núcleo frio | Referência 128×128; escala do protótipo revisada |
| Bram / Lyra / Irian / Maera / Sera | Avental/forja; frascos; indumentária do conselho; guarda; mercadoria | Silhuetas distinguíveis sem nomes flutuantes permanentes |

A célula do boss 128×128 substitui a referência inicial de 96×96 para esta proposta do protótipo. Esses tamanhos são metas de desenho, não prova de sprites produzidos. A escala lógica 32 px/m não obriga um humano de 1,8 m a ter sprite de 57,6 px.

## 4. Cenário e atmosfera

**Cidadela:** pedra azul-cinza, metal gasto, tecidos marfim, lanternas âmbar e um cristal ciano. Mostrar trabalho e abrigo: oficina, bancada de frascos e mercadorias agrupadas. O hub pode deteriorar narrativamente sem perder circulação ou serviços.

**Bosque:** musgo dessaturado, troncos retorcidos, passagens claras e ruínas fragmentadas. Névoa em camadas descontínuas nas bordas, com corredores visuais livres em torno dos inimigos. Pequenos focos violetas sugerem contaminação; não cobrir todos os objetos com brilho.

**Aldeia abandonada:** telhados quebrados, marcas de ocupação recente, um espaço de captura reconhecível e pistas interativas pontuais. A composição deve sustentar investigação antes da decisão; não revelar consequências pelo código de cores “bem/mal”.

## 5. HUD e controles

Topo esquerdo: retrato do Bastião e barras HP/MP/stamina diferenciadas por ícone e rótulo. Topo direito: objetivo curto e recolhível. Centro inferior: cinco slots com teclas **1 · 2/Q · 3/E · 4/R · 5/F**; dash separado com símbolo de Espaço e cooldown.

Os slots são ações, não ícones fixos de armas. A configuração da classe define conteúdo. No runtime atual, somente básico e pesado estão implementados; a imagem conceitual pode mostrar slots futuros como inativos, sem simular quatro magias entregues.

Painéis em azul-noite, borda dupla prata/marfim, ornamentos apenas nos cantos. Texto pt-BR, foco de teclado evidente e tamanho ajustável. Números de dano opcionais, breves e sem sobrepor o objetivo. Diálogo aparece em caixa inferior com retrato 64×64 e duas alternativas quando se trata do dilema da quest; menus operacionais não têm essa restrição.

Viewport artístico de referência: 640×360, nearest e preferência por escala inteira. A cena técnica permanece 960×540 nesta entrega. Separar resolução do mundo e da UI quando necessário para legibilidade; não prometer pixel-perfect em toda tela sem ajuste/teste.

## 6. Animação e efeitos

Movimento começa e termina com silhueta clara; escudo/machado não trocam de mão por espelhamento automático. Windup comunica compromisso, ACTIVE produz um arco curto e RECOVERY mostra peso. O domínio define duração e dano; a animação acompanha.

Luz em áreas pequenas; partículas contidas; impacto curto; sem bloom que destrua clusters de pixels. Éter é luminoso, não uma camada branca sobre a cena. Boss deve antecipar ataques com postura, chão e som. O modo reduzido remove flashes e tremor sem apagar sinais essenciais.

## 7. Pacote de produção proposto

1. Tileset Cidadela/Bosque: chão, bordas, paredes, árvores, portas e obstáculos.
2. Bastião: idle, andar, básico, pesado, defesa, dash, dano e morte.
3. Lobo/Cultista/boss: idle, deslocamento, antecipação, ataque, dano e morte.
4. UI: barras, cinco slots, dash, estados de indisponibilidade e caixa de diálogo.
5. Efeitos essenciais: antecipação, arco de golpe, impacto, projétil e marcador de interação.
6. Retratos e ambientação de áudio mínimos, após o loop funcionar.

Entregar cada asset com arquivo-fonte, dimensões, pivô, metadados de animação e proveniência/licença. As [imagens da galeria](../media/concepts/README.md) são referências de composição; não é tilemap, atlas, export do Godot ou asset pronto para produção.

## 8. Briefing da imagem conceitual

**Formato:** panorâmico 16:9, uma cena contínua semelhante a um quadro de gameplay idealizado, com acabamento pixel art e HUD discreto. Não fotorealista: “foto do game” é interpretada como visualização da aparência pretendida.

**Cena:** Bastião de armadura azul-ferro, escudo âmbar e machado atravessa ruínas à saída da Cidadela em direção ao Bosque das Névoas; lanternas quentes no portão; lobo etéreo adiante; raízes de Garras de Musgo sugeridas ao fundo; bruma violeta lateral e caminho legível.

**Restrições:** personagens originais; nenhum logo de outra franquia; nenhum shader que esconda pixels; sem números de desempenho ou textos afirmando build final. HUD com cinco slots numerados, os três últimos inativos. Pequena identificação “CONCEITO VISUAL” para distinguir proposta de captura real.

**Prompt de geração:**

> Use case: stylized-concept. Create a polished original 16:9 pixel-art concept frame for the dark-fantasy action RPG Echoes of Aethel. This is a visual target, not an actual gameplay screenshot. Top-down three-quarter camera, simulated 2:1 depth, crisp deliberate 16-bit pixel clusters, coherent tile-scale world, restrained atmospheric lighting. A small readable Bastion warrior in blue-iron armor, weathered cloth, amber shield and heavy axe stands in the lower central third on a clear worn stone path. To the left, the stone gate of the Last Citadel with warm amber lanterns and subtle cyan ether crystal provides a safe visual anchor. To the right and deeper into the frame, the Forest of Mists: dark moss, ancient broken masonry, gnarled trees and sparse violet fog along the edges. An ethereal wolf with a cold luminous silhouette waits visibly ahead, separated from the warrior; far in the forest a large root-and-stone silhouette hints at the Moss Claws guardian. Keep walkable ground and the wolf unobscured. Strong silhouettes, modest ornament, quiet melancholy, layered natural materials. Palette navy #17213B, charcoal #141820, stone #555866, moss #526044, ivory #DDD5BD, amber #D9A441, cyan #91D5DE, violet #A17AC4. Discreet dark-blue HUD with double silver/ivory borders: top-left small hero portrait and three simple colored resource bars; bottom center five square action slots labeled 1, 2/Q, 3/E, 4/R, 5/F, the last three inactive; separate dash slot labeled ESPACO. Small elegant title ECHOES OF AETHEL and a small label CONCEITO VISUAL. Prioritize the environment over UI. No photorealism, no first-person camera, no glossy 3D, no giant foreground portrait, no borrowed franchise characters, no heavy bloom, no fog covering combat, no fake performance counters.

## 9. Aceite visual

O Bastião, o lobo, o chão caminhável e a ameaça distante são distinguíveis em tamanho reduzido. A luz guia o percurso; UI não ocupa o centro do combate. O quadro transmite segurança de um lado e contaminação do outro. A implementação posterior precisa ser comparada com a proposta dentro da engine, em movimento e com efeitos reduzidos.
