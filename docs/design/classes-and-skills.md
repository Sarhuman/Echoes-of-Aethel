# Classes e árvores de habilidades

Design inicial, sem balanceamento validado. IDs de classe permanecem os do GDD. Nomes clássicos são descrições, não substituem os nomes próprios.

## Atributos e recurso

Força: dano físico pesado. Destreza: precisão e mobilidade. Inteligência: dano arcano. Fé: fogo sagrado e proteção. Vitalidade: vida e resistência. Vontade: invocações, controle e sustentação. Sabedoria é o nome narrativo da especialização de Vontade da Invocadora; não cria atributo duplicado. Precisão é estatística derivada; Éter é recurso, não atributo.

Recurso comum do protótipo: Éter, exibido como MP. Cada classe poderá ter apresentação temática própria, preservando o mesmo contrato de custo. Nenhuma classe depende de Fé para poções ou de Inteligência para esquivar.

## Estrutura compartilhada

Ataque básico, defesa utilitária e esquiva acessíveis no início. Três ramos por classe, cada um com uma habilidade ativa e uma passiva inicial. Desbloqueios propostos nos níveis 2/4/6; um ponto por nível a partir do 2; cada nó custa um ponto. Ativa do ramo precede sua passiva. Sem ranks no primeiro desenho. No máximo três ativas equipadas além do kit básico. Respec gratuito no hub durante o protótipo; escolhas narrativas não são resetadas.

As especializações ampliam opções; nenhuma é requisito para passar por um mapa. Fórmulas, custos e cooldowns virão após medir o combate básico.

| Classe / identidade visual | Kit básico | Ramo 1: ativa → passiva | Ramo 2: ativa → passiva | Ramo 3: ativa → passiva |
|---|---|---|---|---|
| Tecelão / mago; manto violeta, runas claras | Projétil rúnico; barreira curta; esquiva | Lança de Geada → Fratura Rúnica, controle melhora próximo acerto | Círculo de Brasas → Cinzas Vivas, queimadura limitada | Nó de Éter → Memória do Selo, barreira melhora recuperação |
| Sentinela / ladina; capa escura, adagas prateadas | Corte de adaga; aparo; esquiva | Passo Velado → Última Sombra, saída da ocultação fortalece um golpe | Armadilha de Agulhas → Sangue Frio, efeito de sangramento limitado | Ruptura Tendínea → Caçadora Silenciosa, bônus contra alvo controlado |
| Bastião / guerreiro; armadura azul-ferro, escudo âmbar | Golpe de machado; guarda; esquiva | Impacto do Baluarte → Postura Firme, resistência após defesa | Grito do Crepúsculo → Voto de Guarda, proteção após atrair inimigos | Fenda de Aço → Peso do Juramento, golpe pesado reforçado |
| Invocadora / druida; musgo e osso, cajado ramificado | Espinho lançado; raízes defensivas; esquiva | Golem de Pedra → Raiz Mineral, resistência da invocação | Fera Ancestral → Elo da Matilha, bônus limitado por vínculo | Jardim de Pragas → Ciclo Amargo, dano contínuo com limite de stacks |
| Inquisidor / híbrido; marfim, cobre e chama azul | Maça; selo protetor; esquiva | Martelo de Cinzas → Brasa Persistente, queimadura | Chama de Vigília → Fé Ferida, autocura limitada durante combate | Veredito Solar → Última Confissão, bônus contra mortos-vivos |

## Limites de implementação

Ocultação quebra ao atacar e não apaga a posição conhecida instantaneamente. Controle em bosses reduz eficácia e não permite stun infinito. Invocações têm teto de entidades e reaproveitam navegação validada. Cura não ultrapassa vida máxima nem cria loops de regeneração com custos de habilidade. Provocação afeta IA, sem prometer um sistema de grupos online.

Custos narrativos e passivas somam por regras explícitas, com teto e expiração. Bônus ofensivos não anulam o custo da própria habilidade sem regra prevista. Todas as classes precisam de ataque gratuito utilizável quando MP acaba.

## Escopo real

Somente Bastião e kit básico entram na primeira fatia. As outras classes e todos os ramos acima são especificações futuras. Aceite posterior: cada classe consegue concluir o Ato I, com testes de custo/efeito e leitura visual distinta a 1×.
