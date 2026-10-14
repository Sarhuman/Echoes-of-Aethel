# Autoria de mapas com Tiled

Tiled é a ferramenta escolhida para desenhar os próximos mapas. A cena Godot continua responsável por combate, câmera, iluminação e interação. Cidadela e Bosque desta fatia ainda são cenas Godot com fundo pintado; não são mapas Tiled e não devem ser descritos como tal.

Para cada mapa novo, mantenha o arquivo-fonte `.tmx` e seus tilesets no repositório. Use orientação isométrica 2:1, tamanho de tile fixo por conjunto e coordenadas em pixels para os objetos. Separe chão, obstáculos, elementos acima dos atores e detalhes decorativos em camadas. Use camadas de objetos para colisão, spawn, saídas e pontos de interação, com IDs estáveis compatíveis com o catálogo do jogo e com o save.

O exportador para Godot deve validar IDs duplicados, destinos inexistentes e objetos obrigatórios antes de gerar a cena. A cena gerada não deve conter estado persistente autoritativo: progresso de quest, recompensas e checkpoint permanecem nos serviços existentes. Inclua no PR de cada mapa a fonte Tiled, a cena gerada, teste de navegação e captura real do jogo. Não edite manualmente a cena gerada sem reproduzir a mudança no `.tmx`.

Antes de migrar Cidadela ou Bosque, faça um mapa pequeno de prova com uma saída, uma colisão e um spawn. Só substitua as cenas atuais após comparar enquadramento, profundidade visual e percurso jogável.
