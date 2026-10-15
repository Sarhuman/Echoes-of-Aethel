# Elenco e bestiário para produção 3D

**Estado:** índice visual de pré-produção, sem modelos, rigs ou IA prontos. Os cinco heróis e habilidades vêm do [GDD 01](../GDD/01_classes_e_atributos.md); os encontros por mapa vêm do [GDD 04](../GDD/04_mapas_e_quests.md). Descrições de forma e movimento abaixo são **NOVO/EXPERIMENTAL** e não introduzem dano, HP, habilidades ou drops novos. Não derivar IDs técnicos dos nomes desta página.

## Heróis e figuras recorrentes

| Papel | Silhueta e animação a preparar | Continuidade |
|---|---|---|
| Bastião `twilight_bastion` | Armadura pesada azul-ferro, machado e escudo; peso nos pés, recuo de bloqueio, passo curto antes do golpe | Guarda/Impacto/Fenda seguem GDD 01 com MP conforme decisões da primeira fatia |
| Tecelão `rune_weaver` | Manto estreito, cajado e runas separadas do corpo; antecipação com mão e foco | Projétil, geada, brasas e barreira precisam de leitura distinta |
| Sentinela `shadow_sentinel` | Duas adagas assimétricas e capa curta; centro baixo, passadas rápidas sem deslizar | Ocultação não torna a IA automaticamente esquecida |
| Invocadora `thorn_summoner` | Cajado ramificado, osso e musgo; gesto separado do deslocamento | Golem e fera têm corpos, colisores e limite próprios |
| Inquisidor `sacred_flame_inquisitor` | Tabardo marfim/cobre, maça e fogo azul; gesto de cura diferente do golpe | Dano sagrado e cura não criam imunidade narrativa |
| Irian `npc.elder` | Arquivo, cristal e conselho; postura reservada, mãos nos registros | Evidência da genealogia aparece antes de Vael |
| Bram `npc.blacksmith` | Ferreiro de oficina; braços marcados por trabalho, leitura clara na praça | Serviço essencial permanece acessível após escolhas |
| Lyra `npc.alchemist` | Cura e água; frascos e tecido de enfermaria | Triagem/poço variam por flag sem desativar cura |
| Maera `npc.gatekeeper` | Guarda no portão, silhueta rígida e gesto de abrir passagem | Reação à aldeia muda fala, não bloqueia a viagem |
| Sera `npc.merchant` | Comércio e caravanas; bolsa, livro de preços, banca reconhecível | Loja essencial continua disponível em escassez |

Outros NPCs de cada quest e seus nomes/IDs ficam no GDD 04/07; esta tabela não os substitui. Os cinco heróis são **alternativas de protagonista**, não um grupo que viaja junto. Compartilhar esqueleto é decisão técnica futura; variação de silhueta e animação precisa sobreviver a qualquer reuso.

## 33 entradas de inimigo comum

O GDD lista três tipos nos mapas 01–11; o 12 não tem comuns. **33 entradas de encontro não equivalem a 33 rigs exclusivos.** Família aqui é linguagem de produção, não taxonomia racial ou regra de facção. Toda criatura precisa de idle, deslocamento com contato no chão, antecipação, ataque, reação e morte/desaparecimento sem resíduo visual persistente. Cápsula, separação e caminho de retorno devem impedir o travamento observado no protótipo Godot.

| Mapa | Comuns do GDD | Forma e movimento propostos para distinguir ameaças |
|---|---|---|
| 01 | Lobo Etéreo; Cultista Cego-Guiado; Espectro de Folhas | Quadrúpede translúcido de salto curto; humano que tateia guiado por marcas; folhas que se reúnem em corpo antes de atacar |
| 02 | Larvas Parasitas; Afogados de Éter; Harpias de Lama | Corpo baixo segmentado; humano encharcado com arrasto pesado; ave/figura alada que pousa antes do golpe |
| 03 | Defensores Rúnicos; Cultistas Fanáticos; Cães de Guerra | Armadura/selo que desperta; humano agressivo de postura frontal; quadrúpede treinado e não etéreo |
| 04 | Escorpiões de Cristal; Nômades Corrompidos; Gárgulas de Areia | Pinças e cauda com aviso; pessoa com equipamento de travessia e sinais de Bruma; estátua que desce ao piso para combater |
| 05 | Ritualistas de Sangue; Harpias de Obsidiana; Hienas de Éter | Humano que prepara selo visível; asas angulosas e pouso duro; quadrúpede de corrida lateral |
| 06 | Esqueletos Cristalizados; Múmias Rúnicas; Sombras Devoradoras | Ossos rígidos que estalam; corpo envolto de faixa com gestos lentos; sombra com núcleo visível que anuncia alcance |
| 07 | Ratos de Éter; Mutantes de Esgoto; Aranhas-Luminosas | Enxame baixo limitado; humano deformado ainda legível como pessoa; artrópode de brilho que marca sua posição |
| 08 | Constructos de Bronze; Autômatos; Elementalistas de Magma | Corpo fundido de impacto pesado; máquina articulada de ritmo repetível; conjurador envolto em calor, distinto de poça ambiental |
| 09 | Abominações de Carne; Vermes de Éter; Parasitas Espirituais | Massa assimétrica com pés/apoio visíveis; corpo escavador com emergência sinalizada; forma aérea pequena com área de contato clara |
| 10 | Anjos Caídos; Sentinelas de Prata; Espectros Celestiais | Silhueta alada quebrada; armadura vertical de guarda; figura luminosa com núcleo/sombra legíveis em fundo claro |
| 11 | Guardas de Vael; Constructos Rúnicos Supremos; Manifestações da Bruma | Humano disciplinado; constructo maior e com ritmo próprio; Bruma condensada com borda opaca para leitura |

**Drops:** não atribuir item exclusivo a cada linha acima. Equipamento, consumíveis, materiais, essências, ouro e XP seguem as categorias e probabilidades do [GDD 03](../GDD/03_sistema_de_loot.md). Dentes, bronze, água, runas e outros materiais citados no GDD 04 são intenção temática até catálogo econômico versionado. Inimigos não podem conceder recompensa no início da animação de morte.

## Confrontos de fim de mapa

| Mapa | Papel canônico | Leitura visual e arena proposta; mecânicas exatas pendentes |
|---|---|---|
| 01 Garras de Musgo | Subchefe | Corpo de madeira/raiz, patas apoiadas e sombra de contato; arena com espaço de desvio e raízes sinalizadas |
| 02 Matriarca das Águas | Subchefe | Massa aquática com centro visível; poças distintas da água decorativa |
| 03 Silvano | **Boss do ato I** | Guardião de raiz e armadura; centro do templo e fragmento legível depois da vitória |
| 04 Guerreiro de Vidro | Subchefe | Placas translúcidas e pontos de fratura; não desaparecer contra areia clara |
| 05 Executor de Bronze | Subchefe | Silhueta vertical de sentença, arma pesada e área de impacto inequívoca |
| 06 Malignus | **Boss do ato II** | Corte de ossos e máscara; almas e selo visíveis sem poluir telegraphs |
| 07 Mãe das Proles | Subchefe | Corpo central e crias limitadas; quantidade de auxiliares a testar |
| 08 Autômato Forjador | Subchefe | Massa de forja distinta dos autômatos comuns; piso seguro marcado |
| 09 Gorgoroth | **Boss do ato III** | Boca/corpo ligado à alvenaria; vínculos com vítimas fora das hitboxes |
| 10 Arconte de Prata | Subchefe | Armadura prateada, asas ou extensão apenas visual até colisão aprovada |
| 11 Guardião dos Portões | Subchefe | Porte de sentinela final, entrada/saída da ponte sempre reconhecíveis |
| 12 Vael | **Boss final** | Figura humana de autoridade que evolui para Sol-Negro; ainda precisa de fases e câmera próprias |

Bosses de ato e subchefes têm categorias de loot diferentes no GDD 03. As dungeons opcionais de [Namar e Gorvak](playable-cast-and-side-dungeons.md) são propostas separadas, fora desses 12 confrontos e do bestiário de campanha aprovado. Seus povos, cultura, encontros e recompensas continuam em revisão editorial.

## Pacote de produção por personagem

Cada ficha definitiva deve conter ID estável, zona e função, escala relativa ao herói, turnaround, paleta/material, rig/animações, pontos de contato, cápsula e hurtboxes, telegraphs, áudio, variante por flags, origem/licença dos arquivos e estado de implementação. Para os heróis, acrescentar equipamento por classe, prólogo, fala de Vael, morte/reload e efeitos reduzidos. Para NPCs, acrescentar serviço, diálogo e posição alcançável em todos os estados do hub. Esta página é índice; nenhum desses pacotes está pronto por existir a tabela.
