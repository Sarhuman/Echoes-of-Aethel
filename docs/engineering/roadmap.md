# Plano de implementação

## Direção após a primeira fatia — 14/10/2026

A referência de sensação é um ARPG sombrio de câmera elevada, com combate de impacto, equipamento legível e mapas conectados. A base atual em Godot 4.6 e arte 2D pintada permite avançar nessa direção; trocar para 3D ou migrar de motor agora exigiria refazer mapas, atores, colisão, UI e export sem resolver automaticamente as falhas de leitura ou impacto. Reavaliar a tecnologia apenas depois de um protótipo comparativo que mostre ganho mensurável de jogabilidade.

Ordem das próximas entregas jogáveis:

1. **Apresentação e narrativa:** abertura curta, primeiro conselho, objetivos claros, menus legíveis e feedback de impacto; manter cenas puláveis e progresso idêntico após pular. Em andamento nesta versão.
2. **Mapa 02 — Pântano dos Lamentantes:** prova pequena em Tiled com arquivo `.tmx`, saída, colisão e spawn; validar câmera, escala 2:1, navegação e distância dos inimigos antes de ampliar o mapa. Névoa e parallax ficam em camadas decorativas, sem mover o chão ou esconder limites de colisão.
3. **Combate no Pântano:** novas famílias de inimigos, ataques com antecipação distinta, reação a acerto e loot próprio. Ajustar efeitos por playtest; a opção Efeitos reduzidos continua desligando tremor e flashes.
4. **Portal do Julgamento e Silvano:** concluir o Ato I com fragmento único, duas rotas narrativas persistentes, retorno à Cidadela e cenas antes/depois do confronto.
5. **Interface e produção:** retratos, codificação visual de itens, navegação por controle/teclado, opções de áudio e escala de UI; export Windows e playtest em resolução/tela diferentes a cada marco.

Não copiar arte, áudio, nomes, mapas ou regras de Diablo II ou Path of Exile 2; as referências orientam ritmo e legibilidade, enquanto o conteúdo é de Aethel.

## 0 — Validar a base técnica escolhida

Definir plataforma inicial e controles. Validar Godot/GDScript, conforme a [Sprint 6](../GDD/06_arquitetura_software.md), com uma cena pequena: movimento, colisão, ataque, luz e save. Registrar decisão, versão verificada e instruções reproduzíveis. Aceite: exportação executável no equipamento-alvo e retorno de save válido.

## 1 — Fatia jogável do Bosque

Cidadela segura, Bastião, Bosque das Névoas, lobos/cultistas, Garras de Musgo, vida/recurso, inventário e loot simples. Quest A Caça ao Espião com duas opções, pistas e retorno ao hub alterado. Arte temporária identificada como tal.

Aceite: sair do hub, concluir encontro e decisão, coletar loot, voltar, observar consequência e carregar o save sem duplicações. Movimento não atravessa paredes; ataque forte do boss é antecipado e evitável; morte permite retry. Nenhum dano ocorre na Cidadela.

## 2 — Completar o Ato I

Pântano, Portal, Silvano e primeiro fragmento. Implementar eventos atrasados, diário, progressão entre mapas e quests secundárias. Aceite: ambas as rotas do ato são concluíveis; respawn e reload preservam escolhas; fragmento único.

## 3 — Diversificar combate

Adicionar as quatro classes restantes uma a uma; testar controle, invocações, custos e equipamentos. Projetar árvores após observar o combate real. Aceite: cada classe conclui o Ato I com identidade própria e sem depender de habilidades de outra classe.

## 4 — Expandir a campanha

Atos II–IV, hubs, chefes, pistas de linhagem, confronto com Vael e dois finais com epílogos dependentes de escolhas. Aceite: referências de conteúdo válidas, distribuição dos cinco fragmentos consistente e todas as rotas concluíveis.

## 5 — Produção e lançamento

Arte final, áudio, acessibilidade, balanceamento, migrações de saves e perfil de desempenho. Confirmar plataforma e metas medidas. Multiplayer, contas, backend, geração procedural e endgame extensivo exigem escopo próprio e ficam fora da primeira entrega.

## Estratégia de validação quando houver código

- Unitários de dano/custos, confirmação de escolha, expiração, loot e recompensas únicas com RNG controlado.
- Integração de decisão → save → reload → efeito atrasado, incluindo repetição de comandos e falha de gravação.
- Validador de catálogos: IDs únicos, referências existentes, duas opções por dilema e rotas de progressão alcançáveis.
- Testes de save corrompido, versão desconhecida e recuperação do backup sem perda silenciosa.
- Playtest das duas escolhas e morte/retry com a mesma seed; regressões de colisão e segurança do hub.

Atualização de 14/10/2026: a primeira fatia tem testes headless, export Windows, CI e playtests locais; resultados atuais estão no README. A campanha completa e o playtest humano do novo prólogo continuam pendentes.

## Trilha de infraestrutura online

Após a fatia offline, implementar contratos da Sprint 2, ownership/idempotência, encontros autoritativos, persistência e testes de concorrência. Só então validar Compose, restauração e carga em VPS. Nenhum snapshot econômico offline é aceito como progresso online. Campanha single-player online não pressupõe multiplayer cooperativo.
