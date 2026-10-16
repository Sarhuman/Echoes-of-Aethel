# Persistência e viagem de dungeons — contrato Unreal

**Estado: contrato de implementação, 14/10/2026.** A versão atualizada dos dois textos enviados substitui a primeira onde divergem. O único código novo derivado dela é `FAethelPlayerStats`, com fórmulas inteiras e limites do GDD 01; **não há save Unreal, portal, loot spawner nem mapa de dungeon executável**. O C++ de exemplo não deve ser copiado como implementação pronta.

A [reconciliação G31](g31-offline-loot-authority.md) separa a autoridade local offline das seções FastAPI/PostgreSQL do GDD 03. A [grade e o baú G21](unreal-grid-stash-contract.md) detalham a propriedade das instâncias sem alterar a regra de concessão única.

## Autoridade e dados

O save Unreal terá versão própria, ID estável de personagem/campanha e envelope de integridade/backup testado. Não ler o JSON Godot diretamente como objeto Unreal. `reward_id` identifica a concessão única, enquanto `quest_id` identifica a missão; ambos são strings do catálogo, não `FName` inferido de Actor, arquivo de mapa ou texto localizado. A chave de unicidade é `(character_id, campaign_id, reward_id)`. Um `completed_quest_rewards` mutável por Blueprint não é a autoridade de inventário.

Cada concessão confirma **numa única transação**: resultado da vitória e RNG/seed, XP/ouro, item instanciado ou reserva persistente se a bolsa estiver cheia, `reward_id`, fatos do diário e versão do estado. O save só passa a mostrar o baú aberto ou a missão concluída após a gravação íntegra. Falha de escrita mantém o estado anterior; retry devolve o resultado já gravado, sem rerrolar. A conversa com Irian/Sera apenas apresenta e marca entrega do resultado existente. Antes de implementar `MarkRewardAsClaimed`, deve existir API que receba concessão/bolsa/reserva e persista tudo atomamente; gravar só o ID depois de spawnar um pickup pode perder o prêmio para sempre.

O drop normal de um subchefe é outra parte do mesmo resultado de vitória: **equipamento garantido**, raridade por pesos inteiros 7500 Raro / 2500 Épico, mais rolagens independentes de ouro, consumível, material e essência do GDD 03. `<= 25.0f` com float não é o contrato de RNG inteiro e introduz borda arbitrária. A recompensa adicional de primeira conclusão tem template e `reward_id` próprios, ainda por catalogar; não conceder Lendário automaticamente. Replay da mesma vitória não gera novo loot/XP. Reentradas para farm exigem regra própria antes de habilitar respawn.

## Viagem e retorno

O ponto de retorno é uma **âncora autoral navegável** (`zone_id`, `anchor_id`, transform validado), não `GetActorLocation()` do portal nem `GetWorld()->GetName()`. A escolha da âncora de entrada é persistida na primeira criação da zona. `C` só inicia viagem se o jogador estiver em alcance e linha de visão, a zona/quest estiver desbloqueada, o destino existir e o estado da campanha corresponder à versão esperada. Interação recusada não altera save nem mundo.

Persistir bilhete versionado `{origin_zone_id, return_anchor_id, dungeon_id, layout_seed, state}` com `state=pending` **antes** de `OpenLevel`. Confirmar `arrived` somente quando o destino tiver carregado e o ponto seguro for válido. Se o processo encerrar entre as duas etapas, recuperar na origem ou no destino seguro conforme o último estado confirmado. Falha de nível preserva o bilhete e oferece retorno; nunca posicionar o herói em coordenada vazia. Sair voluntariamente usa a âncora do bilhete; morrer usa o checkpoint da Cidadela, preservando decisões e recompensas já confirmadas. Entrar numa dungeon não cria checkpoint.

O volume de aviso é apresentação reversível e acessível. Ao se aproximar, mostrar risco e opção de retirada; ao sair/pausar/viajar, restaurar névoa/luz/som. A opção de efeitos reduzidos é preferência do jogador, não uma propriedade fixa do portal. O Actor não escolhe loot, não grava quest e não decide a seed por conta própria.

## Atributos e testes de aceite

`FAethelPlayerStats` valida nível 1–30 e cinco atributos efetivos 1–99 antes de recalcular `MaxHP = 60 + 8×VIT + 6×(L−1)` e `MaxMP = 30 + 4×INT + 2×FTH + 3×(L−1)`. O membro C++ se chama `Intelligence` para evitar colisão com `INT` do Windows; o ID canônico de conteúdo/save continua `INT` e terá mapeamento explícito no serializador futuro. Compilação futura executa `static_assert` para Bastião nível 1 (172/52) e nível 2 (178/55). Mudança de máximo limita HP/MP atuais sem cura automática; distribuição de pontos ainda precisa de serviço e save próprios. Não usar os defaults genéricos da struct como preset do Bastião.

Antes de habilitar viagem/loot no jogo, testes automatizados devem cobrir: entrada fora de alcance; mapa ausente; queda antes/depois do `pending`; carga com âncora obstruída; morte dentro da dungeon; bolsa cheia; falha de escrita; retry da mesma vitória; conversa repetida; dois personagens/campanhas com o mesmo `reward_id`; RNG com pesos 7500/2500; replay sem recompensas adicionais; save adulterado e backup. A base C++ Unreal compilou e o teste Automation de atributos passou em 15/10/2026; os testes de persistência, viagem, loot e dungeon descritos aqui **não foram implementados nem executados**.
