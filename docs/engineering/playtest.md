# Playtest 0.2.0-dev — Bosque / treino do Bastião

## Abrir no Windows ou Linux

1. Use Godot **4.6 stable**, edição Standard (GDScript). Não precisa de .NET.
2. No GitHub, baixe **Code → Download ZIP** e extraia, ou atualize seu clone da main.
3. No Godot Project Manager, clique em Importar e selecione `client/godot/project.godot`.
4. Aguarde a importação e pressione **F5** dentro do editor.
5. Clique em **Iniciar treino** ou pressione Enter.

Não exige VS Code, backend ou login dentro do jogo. Ainda não é um executável Windows independente. A arte da galeria é referência; a arena usa desenhos geométricos provisórios.

## Objetivo e regras

Vença três encontros: Lobo; Lobo + Cultista; dois Lobos + Cultista. O Lobo se aproxima para atacar; o Cultista possui um golpe de alcance maior e preparação mais lenta. Ainda não há projétil ou magia definitiva do Cultista.

A direção do movimento define a mira. O círculo vermelho é o alcance do próximo ataque inimigo; saia dele antes de a preparação terminar. Paredes bloqueiam movimento e dano. O ataque básico não gasta MP; o pesado custa 12 MP. Dash consome stamina.

Entre encontros, avance pelo botão ou Enter: recupera até 30 HP, todo MP/stamina e o cooldown de dash. A vitória encerra a tentativa. F6 ou o botão de resultado iniciam outra. Nenhum loot, XP ou resultado desta arena é persistido.

| Ação | Teclas |
|---|---|
| Movimento | WASD ou setas |
| Ataque básico | 1 |
| Pesado experimental | 2 ou Q |
| Dash | Espaço |
| Iniciar / avançar | Enter |
| Pausar / continuar | Esc |
| Reiniciar tentativa | F6 |
| Habilidades futuras | 3/E, 4/R, 5/F: aviso, sem custo |

F8/F9 continuam disponíveis somente na antiga cena técnica. Para abri-la, selecione `scenes/test_scene.tscn` no editor e execute a cena atual.

## Roteiro de avaliação humana

- Ataque olhando para o inimigo; confirme barra de vida e morte.
- Use uma ruína como cobertura: golpes não devem atravessá-la.
- Faça um inimigo contornar a ruína. Se perder a visão por três segundos, ele abandona a perseguição e volta à patrulha.
- Pause durante a preparação de um golpe: atores devem congelar.
- Gaste o MP e confirme que o pesado deixa de executar; o básico continua disponível.
- Complete as três ondas e confira vitória; reinicie e deixe o jogador morrer para conferir derrota.
- Informe onda, posição, ação, resultado esperado/obtido e screenshot se houver falha.

## Arquitetura e limites

`ArenaNavigation` compartilha uma grade A* imutável com margem de corpo; `ArenaBrain` mantém rota e memória por inimigo, reutilizando a FSM existente. A geometria usada para colisão e navegação nasce da mesma lista de obstáculos. Rotas são recalculadas no máximo a cada 0,25 s para destino estável. Posições sem célula caminhável produzem parada segura. Não há obstáculos dinâmicos nem desvio entre atores, que podem se sobrepor. O desenho da arena apenas lê o estado de combate.

A arena não modifica persistência, dano nem idempotência do controlador existente. A percepção circular já existente na FSM continua sendo usada. O Cultista é uma variação experimental do ataque da FSM, não sua versão final do GDD.

Validação automatizada: 144 verificações no Godot 4.6 Linux headless. Inclui trajetória física em torno da parede, memória sem visão, pausa, encontros, restauração, vitória/derrota e regressões. Não substitui avaliação visual, balanceamento e teste no computador do jogador. Não foi possível validar a janela gráfica neste ambiente.

Referência da API: [AStarGrid2D — Godot 4.6](https://docs.godotengine.org/en/4.6/classes/class_astargrid2d.html).
