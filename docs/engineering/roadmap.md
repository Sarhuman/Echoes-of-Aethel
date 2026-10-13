# Plano de implementação

## 0 — Validar a base técnica escolhida

Definir plataforma inicial e controles. Validar Flutter/Flame, conforme a [Sprint 2](../GDD/02_arquitetura_tecnica.md), com uma cena pequena: movimento, colisão, ataque, luz e save. Registrar decisão, versão verificada e instruções reproduzíveis. Aceite: exportação executável no equipamento-alvo e retorno de save válido.

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

Este PR entrega documentação. Não há testes de execução ou CI de jogo para alegar como aprovados.

## Trilha de infraestrutura online

Após a fatia offline, implementar contratos da Sprint 2, ownership/idempotência, encontros autoritativos, persistência e testes de concorrência. Só então validar Compose, restauração e carga em VPS. Nenhum snapshot econômico offline é aceito como progresso online. Campanha single-player online não pressupõe multiplayer cooperativo.
