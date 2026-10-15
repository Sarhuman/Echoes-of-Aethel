# Contrato de entrada, diário e recompensa das dungeons opcionais

**Estado:** `NOVO/EXPERIMENTAL`. Especificação portável para a futura implementação Unreal; existe `.uproject` e infraestrutura de apresentação, mas não há C++ compilado nem dungeon jogável. O [contrato de persistência e viagem](unreal-persistence-contract.md) detalha a transação exigida antes de implementar portal/loot. A [ficha de elenco e dungeons](../design/playable-cast-and-side-dungeons.md) define Namar e Gorvak. O [GDD 03](../GDD/03_sistema_de_loot.md) rege raridade/XP; o [GDD 04](../GDD/04_mapas_e_quests.md) rege os 12 mapas principais. A interação `C` e o diário `J` já são controles aprovados no cliente Godot.

## Descoberta e entrada

`AAethelDungeonEntrance` pode ser a **apresentação** de uma entrada, mas sua classe não deve decidir sozinha quest, seed, loot ou viagem. Colocar entradas somente em **âncoras autorais validadas** do mapa permitido: A Coroa de Sucata em `dunes_04`; O Livro dos Nomes Perdidos em `crypt_06`. Quando houver várias âncoras, selecionar uma com seed de campanha persistida **na primeira criação da zona**, nunca em cada `BeginPlay` ou reload. Se houver apenas uma âncora, a entrada é fixa. Não sortear livremente coordenadas, pois pode bloquear caminho, surgir atrás de parede ou revelar a entrada antes da pista.

Ao apertar `C`, o controlador procura um único alvo interagível em alcance e linha de visão, mostra nome e risco e envia `RequestEnterOptionalDungeon(entrance_id, expected_state_version)`. O serviço valida zona atual, descoberta/quest, distância, estado do personagem, dungeon desbloqueada e disponibilidade do destino. O nome mostrado do subchefe é dado de localização; não é autoridade para escolher o nível. Interação recusada não salva posição nem abre mapa.

Após validação, persistir um **bilhete de transição**: origem (`zone_id` + `anchor_id`/transform validado), dungeon, seed/layout, versão e estado `pending`. Só então iniciar a viagem. Em falha de carregamento, manter o jogador na origem e recuperar o bilhete; em fechamento do jogo durante a transição, o load retoma na origem ou no destino seguro conforme estado confirmado, nunca em coordenada vazia. A saída da dungeon usa o bilhete confirmado e devolve à âncora de retorno livre de colisão. Morte normal aplica o checkpoint seguro já definido, preservando quest/loot confirmados; entrada de dungeon não cria checkpoint automaticamente.

## Diário (`J`) e missão secundária

O diário apresenta abas **Campanha** e **Secundárias**. Cada entrada usa `quest_id` estável, título localizado, objetivo atual, local conhecido, pista observada, risco recomendado e recompensa **conhecida**. Conteúdo não observado permanece oculto. O diário lê a projeção do estado persistido; abrir/fechar a tela não inicia quest nem concede recompensa.

Estados propostos: `hidden → rumored → available → active → cleared → claimed`. `rumored` surge com a fala/pista de Irian ou Sera; `available` requer acesso à zona; `active` começa na aceitação; `cleared` vem da vitória confirmada do subchefe; `claimed` vem da recompensa/retorno confirmado. Repetir evento devolve o mesmo estado. A entrada pode ser explorada antes de aceitar a missão **se** a pista e a zona já estiverem disponíveis; o diário então registra descoberta, e a futura aceitação aproveita a vitória existente sem matar o chefe outra vez. Uma escolha secundária, se adicionada, ganha flag própria e não altera carga ritual nem gate principal por acidente.

Contrato mínimo de conteúdo por quest: `quest_id`, `dungeon_id`, `source_zone_id`, `allowed_anchor_ids`, `unlock_condition`, `giver_npc_id`, `discovery_fact_id`, `boss_id`, `objective_ids`, `first_clear_reward_id`, `return_anchor_id`, `journal_text_keys`. IDs finais devem passar pelo validador de referências antes de criar `Actor`, Data Asset ou save. A posição física não é ID de domínio. Quest principal e secundária usam o mesmo mecanismo de persistência, com categorias separadas apenas na apresentação.

## Derrota, loot e economia

Ao derrotar Namar ou Gorvak, confirmar a morte do encontro **uma vez** e usar a categoria **sub-boss** do GDD 03: um equipamento garantido, com raridade **75% Raro / 25% Épico**, além das rolagens definidas de ouro, consumível, material e essência. Não chamar `DropGuaranteedRareItem()` no Actor nem conceder Lendário automaticamente; isso ignoraria o catálogo, a transação e a idempotência. A recompensa adicional de primeira conclusão da **quest** precisa de template e orçamento próprios. XP usa categoria sub-boss e base por zona ainda a simular; anunciar “mais XP/ouro” apenas após números medidos, sem criar farm infinito por reload.

Fluxo autorizado: vitória validada → resultado de loot/XP + `boss_defeated` + `first_clear_reward_id` em uma transação ou reserva persistente → diário `cleared` → animação/números/baú. Se a bolsa estiver cheia, a recompensa vai à reserva. Uma nova tentativa da mesma vitória devolve o resultado original. A conversa de entrega marca `claimed` e mostra o resultado existente, sem rolar novamente. Replay e respawn de dungeon exigem política separada; a primeira conclusão não pode ser obtida de novo.

## Sinalização visual e acessibilidade

Na proximidade da entrada, usar volume de apresentação local para interpolar névoa/luz/som: violeta e ecos ósseos para Namar; cobre/vermelho e ferragens para Gorvak. O efeito comunica perigo antes da interação, sem alterar a iluminação do mapa inteiro, colisões ou IA. Ao sair do volume, viajar, morrer ou pausar, restaurar o estado anterior. Modo de efeitos reduzidos diminui pulsação/flash, mantendo placa, ícone e texto de risco; nunca depender apenas de cor. O painel de entrada diz que a área é opcional, informa retirada e apresenta recompensa conhecida sem prometer raridade não garantida.

## Critérios de implementação

1. Entrada aparece em âncora permitida e continua na mesma posição após save/reload; nenhuma entrada nasce em piso inacessível, poço ou frente de portão obrigatório.
2. `C` fora de alcance ou com destino indisponível não viaja nem modifica save. Falha de carregamento e reinício no meio da transição oferecem retorno seguro.
3. Sair voluntariamente e morrer devolvem ao lugar correto pelas regras de cada caso, sem perder decisões, ouro ou itens confirmados.
4. `J` separa campanha/secundárias e revela somente fatos observados. Vitória antes da aceitação não duplica chefe nem recompensa quando o NPC atualiza a missão.
5. Vitória, reload, conversa repetida e bolsa cheia mantêm XP, ouro e item exatamente uma vez; taxa Raro/Épico do sub-boss usa RNG controlado em teste.
6. Volume visual é reversível e legível com efeitos reduzidos; performance e contraste são testados no PC alvo.

O trecho C++ enviado pelo usuário é ponto de partida conceitual. `print` não é a saída de log C++ da Unreal, e `OpenLevel` direto não faz as validações nem a persistência acima. Implementar gateway/loot somente com serviço transacional, destino/âncora validados e testes de automação; compilar na Unreal instalada antes de habilitar a viagem.
