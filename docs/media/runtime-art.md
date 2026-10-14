# Arte e áudio da primeira fatia jogável

As seis imagens em `client/godot/assets/painted/` foram geradas para Echoes of Aethel em 13–14/10/2026 com a ferramenta imagegen. O pedido usou [a imagem conceitual fornecida pelo usuário](concepts/aethel-visual-target.png) apenas como referência de paleta, perspectiva e atmosfera; os mapas, personagens e itens são novas composições. Foram copiadas para o projeto como PNG originais, sem bancos de assets de terceiros. O proprietário ainda não definiu uma licença geral para o código e a arte.

| Arquivo | Uso no jogo |
|---|---|
| `citadel_courtyard.png` | cenário explorável da Cidadela |
| `forest_village.png` | Bosque, aldeia, passagem e arena |
| `actors_atlas.png` | Bastião, Lobo Etéreo, Cultista Cego e Garras de Musgo |
| `items_atlas.png` | ícones de armas, armaduras, consumíveis e materiais |
| `bastion_walk.png` | quatro quadros de caminhada do Bastião |
| `bastion_attack.png` | quatro quadros de ataque com machado |

O código seleciona regiões dos atlas em tempo de renderização; os PNGs permanecem intactos. Os arquivos em [runtime/](runtime/) são **capturas reais do Godot** desta versão e mostram Cidadela, Bosque, arena, ataque e bolsa. As três imagens em [concepts/](concepts/) continuam sendo apenas referências de direção.

Os tons de interface e ataque são sintetizados em `ui/hud/hud.gd` e ainda precisam de composição e mixagem próprias. A fonte é o fallback fornecido pela engine Godot.
