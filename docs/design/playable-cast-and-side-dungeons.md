# Elenco jogável e dungeons secundárias — proposta de produção 3D

**Estado:** `NOVO/EXPERIMENTAL`, 14/10/2026. Esta ficha amplia o planejamento, não afirma que há classes, masmorras ou chefes 3D implementados. História, classes, economia, save e dificuldade seguem os GDDs 01, 03, 04, 05 e 07. Números e IDs novos só entram no catálogo após simulação e playtest.

## Quem entra em cena e quem pode ser escolhido

O jogador escolhe **um** dos cinco descendentes de Vael ao iniciar uma campanha. Todos percorrem os mesmos 12 mapas e enfrentam os mesmos dilemas principais; prólogo, algumas falas, reações a Vael, equipamentos e soluções táticas variam por classe. Não existe grupo simultâneo de cinco heróis nem troca livre de classe no meio de uma campanha. Personagens de outras classes não surgem automaticamente como acompanhantes ou NPCs alternativos: isso exigiria cenas, continuidade e regras próprias.

| Herói do GDD 01 | Leitura visual para modelagem 3D, preservando a identidade original | Jogo distinto | Estado |
|---|---|---|---|
| Bastião do Crepúsculo (`twilight_bastion`) | Silhueta larga, armadura azul-ferro marcada, machado curto e escudo âmbar; pés e golpe sempre ancorados no terreno | Avanço protegido, Guarda, Impacto e Fenda | Único herói jogável em Godot; primeira prova Unreal |
| Tecelão de Runas (`rune_weaver`) | Manto violeta estreito, cajado com ponta clara, runas de marfim que respondem ao gesto | Projétil, geada, brasas e barreira; combate de distância/controle | Especificado, sem runtime próprio |
| Sentinela das Sombras (`shadow_sentinel`) | Capa azul-noite, capuz recortado e duas adagas assimétricas; ocultação lida pela silhueta, não por invisibilidade absoluta | Mobilidade, armadilha, sangramento e abertura furtiva | Especificada, sem runtime próprio |
| Invocadora de Espinhos (`thorn_summoner`) | Cajado ramificado, musgo/osso e espinhos brancos; invocações com corpos e colisores separados | Golem, área venenosa e fera, com limites claros de invocação | Especificada, sem runtime próprio |
| Inquisidor da Chama Sagrada (`sacred_flame_inquisitor`) | Tabardo gasto marfim/cobre, maça larga e fogo azul; símbolo religioso original | Maça, dano sagrado e autocura | Especificado, sem runtime próprio |

Bram (ferreiro), Irian (arquivo/conselho), Lyra (cura), Maera (portão) e Sera (comércio) são **NPCs interativos, não classes selecionáveis**. Devem aparecer na Cidadela e no prólogo; variantes de fala e cenário acompanham escolhas sem eliminar serviços essenciais. As cinco classes têm falas específicas no prólogo e no confronto com Vael no GDD 07, mas só o Bastião pode aparecer como protagonista da primeira prova 3D até que os outros quatro tenham modelo, rig, animações, habilidades, itens e testes.

**Ordem de entrada:** Bastião na rota Cidadela–Bosque; depois Tecelão para validar projéteis/áreas, Sentinela para armadilhas, Invocadora para IA de aliados e Inquisidor para cura/dano sagrado. Antes de anunciar uma classe como jogável, completar campanha de prova do início à volta ao hub, inventário/equipamento compatível, morte/reload, cutscene com variante, e confronto em que seu A1 gratuito funciona. Não liberar classe apenas porque sua ficha existe.

## Política para dungeons fora dos 12 mapas

Os 12 mapas e seus quatro fragmentos continuam a campanha principal. As duas entradas abaixo são **quests secundárias opcionais**, fora dessa contagem, com inimigos mais exigentes do que a rota próxima: composição de elites, telegraphs novos e arena que testa posicionamento. Nenhuma dungeon bloqueia o mapa seguinte, concede fragmento do Coração, repete um dilema principal ou ameaça a segurança da Cidadela. A missão pode ser aceita antes da entrada e concluída por retorno ao contratante.

A [especificação de entrada, diário e recompensa](../engineering/optional-dungeon-contract.md) define a descoberta em âncoras válidas dos mapas, interação `C`, diário `J`, retorno seguro e loot transacional. Posição aleatória só pode variar entre âncoras autorais com seed persistida; os doze mapas de campanha não dependem de geração procedural.

| Proposta e acesso | Fantasia e desafio | Confronto secundário | Recompensa e consequência |
|---|---|---|---|
| **O Livro dos Nomes Perdidos** (`NOVO/EXPERIMENTAL`) — entrada opcional junto à Cripta de Ossos, após acesso ao mapa 06; Irian oferece a pista | Ossuário lateral com inscrições que apagam nomes, salas de ecos e mortos ligados a sinos. Encontros de mortos-vivos/constructos com pausas de leitura; inscrições expõem fraquezas antes da arena | **Namar, o Rei sem Nome**, um rei-lich original que conserva memórias em placas ósseas. Usa invocações limitadas, círculos de silêncio bem marcados e uma janela segura após cada comando | XP e ouro acima de uma missão secundária comum de mesmo ato; primeira conclusão garante um item raro compatível com a classe ou alternativa declarada no catálogo. Irian recupera ou lamenta nomes conforme a resolução, sem reescrever Malignus ou seu fragmento |
| **A Coroa de Sucata** (`NOVO/EXPERIMENTAL`) — fortaleza lateral nas Dunas de Sal após acesso ao mapa 04; Sera apresenta as caravanas desaparecidas | Passagens de andaimes, barragens de sucata e patrulhas goblins/orcs da **Coligação das Cinzas**. Goblins operam engenhos e sinais; orcs defendem corredores com escudos e avanço coordenado. Evitar “raça inteira maligna”: a coligação tem disputa por água e carga | **Gorvak, Rei das Cinzas**, líder orc com engenheiros goblins. Derruba coberturas sinalizadas, chama no máximo um grupo de reforço por fase e fica exposto após o golpe de marreta | XP, ouro recuperado da caravana e equipamento/material útil antes do mapa 05; uma recompensa fixa de primeira conclusão. Sera altera sua fala conforme a carga recuperada e o tratamento dado aos sobreviventes, sem retirar a loja |

“Rei-lich” é arquétipo de fantasia pedido para a dungeon, **não o nome próprio Lich King** de outra franquia. Namar, Gorvak, a Coligação, lugares, IDs provisórios e recompensas específicas acima são propostas originais; precisam de revisão editorial e de catálogo antes de se tornarem cânone. Orcs e goblins são novos povos/facções para Aethel, ausentes do bestiário principal atual; a produção deve lhes dar origem, cultura, silhuetas, animações, drops e falas próprias antes de prometer encontros.

### Risco, recompensa e persistência

- A dificuldade maior deve ser **visível antes da entrada**: aviso de zona opcional perigosa, faixa recomendada de equipamento como orientação e botão de voltar. Não bloquear por nível do personagem nem depender de um drop aleatório para vencer. Morte normal retorna ao checkpoint seguro e mantém escolhas e itens já confirmados; não cria penalidade oculta de XP nem obriga Eclipse.
- Cada dungeon tem rota de entrada/saída comprovada, ponto de retirada antes da arena, leitura clara de piso perigoso e inimigos que não podem prender o herói contra geometria. Primeira versão com salas autorais fixas. Variação modular futura só após seed e salas persistidas, conectividade garantida, navegação testada e posição de baús/chefes validada.
- Ouro/XP de mortes seguem o contrato de vitória idempotente e categoria do GDD 03. Recompensa de quest e item de primeira conclusão têm `reward_id` único por personagem/campanha. Bolsa cheia usa reserva persistente. Reload, morte ou repetir conversa não duplicam recompensa. Repetição da dungeon para farm só existe se uma política de reentrada for especificada; nunca reaplica a recompensa única.
- Valores exatos de HP/dano, XP, ouro, raridade final, faixa de item, número de salas e tempo alvo ficam `EXPERIMENTAL` até catálogo e teste de economia. O aumento de dificuldade deve oferecer ganho perceptível sem tornar as dungeons obrigatórias para equipar o herói contra chefes da história.
- A conclusão libera uma **microcena pulável** de retorno a Irian ou Sera e uma entrada no diário. Ela reage a fatos já persistidos; assistir/pular não concede itens nem executa a escolha. Uma cena completa com novo cenário só é produzida se o playtest mostrar valor narrativo proporcional ao custo.

## Entrega por etapas

1. Completar e testar a primeira rota 3D com Bastião e NPCs; definir gramática visual, HUD, colisão, loot e save.
2. Fazer blockout de **uma** dungeon fixa (Coroa de Sucata é candidata por chegar antes na campanha), combate do líder e recompensa única. Medir mortes, tempo de viagem, legibilidade, ouro/XP por minuto e retorno seguro.
3. Só após balancear a primeira, produzir Livro dos Nomes e os demais heróis em incrementos verticais completos. O GDD de campanha recebe IDs e recompensas finais antes da implementação; o índice de produção distingue proposto, em construção e jogável.

