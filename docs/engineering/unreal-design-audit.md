# Auditoria de conteúdo e propostas para o cliente Unreal

**Estado:** levantamento de 14/10/2026. Fatos abaixo apontam para o GDD; propostas não alteram o cânone nem descrevem funcionalidades implementadas. A prova 3D ainda não é um jogo Unreal executável.

## Campanha, criaturas e espaços

- O [resumo da campanha](../design/campaign.md) especifica 12 mapas em quatro atos. Os mapas 01–11 listam três arquétipos comuns cada: **33 entradas de mob** no catálogo previsto, não 33 modelos/IA prontos. O mapa 12 contém apenas Vael. Cultistas, mortos-vivos, feras etéreas, parasitas, constructos e manifestações celestiais são famílias visuais/mecânicas úteis; o GDD não define uma taxonomia biológica formal de “raças”.
- Os mapas 03, 06, 09 e 12 têm os quatro bosses de ato: Silvano, Malignus, Gorgoroth e Vael. Os oito confrontos dos outros mapas são sub-bosses. Garras de Musgo é sub-boss do mapa 01.
- Cada bioma, inimigos e recompensas temáticas estão no [GDD 04](../GDD/04_mapas_e_quests.md). A primeira rota 3D propõe oficina de Bram, casa abandonada e porão de raízes, com dois recipientes de suprimento persistentes; são **NOVO/EXPERIMENTAL** até virarem conteúdo jogável.
- Os tamanhos Godot atuais são 1672×941 px (Cidadela) e 1920×1080 px (Bosque). Isso não define quilômetros, metros ou tempo de travessia em Unreal. Definir esses números depois de blockout, teste de câmera, combate, legibilidade das entradas e performance. Não escalar pixels 2D diretamente para metros 3D.
- Proposta visual: geometria transitável e colisões estáveis; árvores, névoa, água, brasas, luz, personagens e objetos narrativos animados; alterações da Cidadela por estágio/flags. Animação nunca muda caminho crítico sem telegraph, navegação válida e teste de retorno. O [GDD 04](../GDD/04_mapas_e_quests.md) já descreve a evolução visual do hub.

## Progressão e recompensa existentes

- O [GDD 03](../GDD/03_sistema_de_loot.md) prevê, além de ouro e XP, equipamento, consumíveis, materiais e essências. Comum tem 12% de chance de equipamento; elite 65%; sub-boss e boss, 100%. Boss de ato concede Épico/Lendário e, na primeira vitória, receita e fragmento únicos. A economia e os afixos ainda são especificação experimental, não conteúdo implementado em Unreal.
- As três quests secundárias do [resumo da campanha](../design/campaign.md) são O Julgamento do Cultista (I), A Água da Vida (II) e A Aliança com o Traidor (III). Elas precisam de contratos de recompensa (ouro/item/XP), pista, acesso e persistência antes de implementação. Proposta: recompensa fixa útil para a preparação do próximo confronto, sem exigir a quest para vencer o boss e sem duplicar recompensa em reload.
- O [GDD 01](../GDD/01_classes_e_atributos.md) prevê nível de personagem 1–30, três pontos de atributo por nível e nós de habilidade compráveis. A curva concreta de XP por nível segue pendente no [GDD 03](../GDD/03_sistema_de_loot.md). Nível de personagem não diminui por morte.
- O [GDD 05](../GDD/05_combate_e_hud.md) **não** prevê seletor Fácil/Médio/Difícil. O desafio sobe pelos atos e por elites em expedições revisitadas; elas usam tabelas de elite para XP/loot. Após terminar a campanha, Eclipse desbloqueia uma **nova campanha/personagem** com permadeath. Na campanha normal, morte retorna ao checkpoint seguro e preserva escolhas. Eclipse não deve ser oferecido antes de implementar e testar suas regras próprias.
- Proposta para acessibilidade a decidir no GDD: assistência optativa na campanha normal, selecionável fora do combate ou após derrota, sem reduzir XP, ouro, drops únicos, decisões ou progresso. Um painel deve explicar exatamente o que muda no combate e mostrar que recompensas permanecem iguais. Não trocar o modo automaticamente nem habilitar a assistência em Eclipse. Qualquer recompensa exclusiva por desafio precisaria de regra separada e transparente.

## Dungeons, cinematics e HUD

- Os 12 mapas autorais abrigam as decisões principais. Interiores e dungeons opcionais podem existir **fora da contagem**. A [ficha de elenco e dungeons](../design/playable-cast-and-side-dungeons.md) propõe duas quests secundárias com chefes originais, ainda `NOVO/EXPERIMENTAL`. Testar primeiro a casa/porão autorais do mapa 01; depois uma dungeon fixa e, só mais tarde, variação modular com seed persistida, rota garantida, colisões válidas e saída segura. Não substituir mapas de história por geração aleatória.
- O [GDD 07](../GDD/07_roteiro_e_dialogos.md) já roteiriza abertura, passagens de ato, apresentação de Vael e finais. Cenas são puláveis e podem ser relidas; pular não altera recompensa, decisão ou combate. Proposta de produção: abertura e grandes marcos de boss/ato recebem sequências curtas; quests secundárias usam câmera/diálogo quando couber. Nenhuma cutscene está pronta em Unreal.
- O HUD Godot atual usa `ProgressBar` retangular e `StyleBoxFlat`. Essa aparência é protótipo, não arte final 3D. Proposta Unreal: vasos de Vida/Éter próprios de Aethel com ornamento de pedra/metal, valores numéricos legíveis, Fôlego discreto e estados claros de dano, custo e recarga. Validar contraste, escala, safe area e versão com efeitos reduzidos. Inspirar a hierarquia visual de ARPGs, sem reproduzir o desenho de Diablo II: Resurrected.

## Revisão dos três textos e dois SVGs recebidos

Os anexos são **referências/propostas**, não instruções para substituir o GDD. A árvore geral `Content`, personagens, input, UI e Sequencer ajuda a organizar a prova, mas o projeto Unreal precisa de `Source/` separado para C++; um SVG não cria `.uasset` ou classe compilada. O diagrama usa “Baluarte” para Bastião e chama Garras de Musgo de boss, contrariando o cânone. “Cemitério” e “Ruínas” do outro diagrama são exemplos genéricos, não os atos de Aethel.

Os exemplos C++ não são código pronto: usam HP/MP hardcoded divergentes do Bastião, multiplicador de flanqueamento de 50% sem regra no combate, `print` como se fosse C++ da Unreal e valores de stamina independentes do catálogo. Restaurar máximos em `BeginPlay` sem integração ao save apagaria estado carregado. Bloquear dano na Cidadela exige regra de domínio/zona, não apenas intercepção visual de input.

O gerador de salas é uma hipótese técnica, não uma descrição verificada do algoritmo de Diablo II. O exemplo usa sorteio sem seed persistida, não garante caminho para boss/saída, não demonstra alinhamento correto dos sockets nem valida navegação/colisão de sala antes de confirmar o spawn. A primeira prova 3D deve manter layout autoral e abordar geração apenas em dungeon secundária após testes automatizados de conectividade e persistência.

## Diferencial de Aethel e ordem de produção

A proposta singular é combinar combate legível de ARPG com escolhas que alteram pessoas, diálogos, arquitetura e encontros, mantendo um hub sempre funcional; cinco linhagens jogáveis ligadas a Vael dão outra leitura à mesma história. Isso é **visão de design**, ainda não experiência entregue. Para a tornar verificável: concluir Cidadela → Bosque em 3D; fazer animação, colisão e feedback de combate; persistir saque/decisão; encenar uma consequência no retorno; só então ampliar bestiário, sistemas de itens, dungeons e os demais mapas. Medir clareza, tempo de travessia, taxa de morte, uso das habilidades, valor do saque e repetição de falhas de navegação em playtest.

