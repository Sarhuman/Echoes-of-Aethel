# Echoes of Aethel — escopo de produto e produção

> Estado de implementação: a primeira fatia está jogável em `0.2.0-dev`. Para conflitos sobre slots, MP, dano e checkpoint, prevalecem as [decisões D1–D8](decisions-first-slice.md). Esta página mantém parte do texto histórico de planejamento.

Versão 1.0 · 13/10/2026 · plataforma inicial: Windows · Godot 4.6.

Este documento consolida o projeto sem substituir os IDs narrativos, o GDD ou os contratos existentes. Define a fronteira da primeira entrega e distingue implementação comprovada de intenção. Controles abaixo foram aprovados pelo usuário; estimativas visuais e números de balanceamento permanecem propostas quando assim identificados.

## 1. O jogo

ARPG single-player offline, dark fantasy em pixel art 16-bit, exploração top-down 3/4 e combate em tempo real. O jogador atravessa um mundo ferido pela ruptura do Coração de Éter. Suas escolhas preservam vidas e valores, mas provocam consequências que reaparecem nos mapas e no hub.

**Promessa:** combate legível e responsivo, preparação de equipamentos e escolhas com consequências observáveis. O peso narrativo vem de personagens e mudanças no mundo; a segurança mecânica do hub nunca é sacrificada para criar drama.

**Público inicial proposto:** jogadores de PC que apreciam RPGs de ação, exploração e decisões narrativas. A primeira validação deve observar entendimento dos controles, leitura dos ataques e interesse pela consequência da quest; não presumir adequação ao público sem playtest.

## 2. Loop central

Preparar-se na Cidadela → explorar o Bosque → reconhecer inimigos e sinais de ataque → lutar usando posicionamento/recursos → obter recompensa → investigar e decidir → retornar à Cidadela → observar consequência → salvar e continuar.

A campanha completa prevê cinco classes, quatro atos e doze mapas. Isso é a visão de expansão, não o conteúdo prometido para o primeiro executável de validação.

## 3. Primeira fatia jogável: Bastião / Cidadela / Bosque

| Área | Incluído no marco | Critério de conclusão |
|---|---|---|
| Personagem | Bastião com movimento, ataque básico, quatro slots de habilidade, dash, HP/MP/stamina | Comandos legíveis, custos consistentes, sem execução ou dano duplicados |
| Hub | Cidadela segura, retorno, interação com serviços essenciais | Nenhum dano/combate hostil; decisões não bloqueiam recuperação/progresso |
| Exploração | Bosque das Névoas, aldeia abandonada e acesso à arena | Percurso contínuo com colisões, entrada/saída e leitura clara do terreno |
| Inimigos | Lobo Etéreo, Cultista Cego e Garras de Musgo | Percepção/ataques legíveis, sem atingir através de paredes |
| Boss | Encontro com Garras de Musgo, antecipações visuais e esquiva testável | Derrota e repetição do encontro possíveis; onda sem depender de precisão não demonstrada |
| Inventário | Coleta, limites de peso/capacidade, equipamentos/consumíveis mínimos | Itens de quest não se perdem por bolsa cheia; capacidade não impede concluir o marco |
| Quest | A Caça ao Espião, na aldeia do Bosque | Executar o suspeito ou poupá-lo e integrá-lo aos Exilados; consequência persistente |
| Persistência | Save/load, backup, morte/checkpoint e recompensa única | Recarregar ou morrer preserva decisões e não duplica nem perde loot/XP |
| Apresentação | Arte provisória coerente, HUD, feedback, som mínimo e opções essenciais | Efeitos não escondem ação; texto legível; controles apresentados dentro do jogo |
| Entrega | Executável Windows e instruções de playtest | Abre fora do editor e permite completar o loop sem console |

As quatro habilidades devem seguir os contratos da classe após reconciliação de custos, duração e prioridades. O pesado técnico atual não aprova sozinho o loadout final. O marco só está completo quando os quatro slots têm funções implementadas e testadas; não confundir teclas reservadas com mecânicas prontas.

## 4. Controles aprovados

| Ação | Tecla primária | Alternativa |
|---|---|---|
| Mover | W / A / S / D | Setas |
| Ataque básico | 1 | — |
| Habilidade 1 | 2 | Q |
| Habilidade 2 | 3 | E |
| Habilidade 3 | 4 | R |
| Habilidade 4 | 5 | F |
| Dash | Espaço | — |

Números são teclas da fileira superior nesta entrega. Os aliases representam a mesma ação; apertar 2 e Q simultaneamente não cria dois comandos. W é exclusivamente movimento. Uma intenção ofensiva por passo de física; básico tem precedência quando múltiplas ações ofensivas chegam juntas. Não há buffer de combo/autoataque nesta etapa.

**Versão jogável 0.2.0-dev:** 1 ou clique esquerdo = básico; 2/Q ou clique direito = Impacto; 3/E = Guarda; 4/R = Fenda; 5/F permanece reservado. Ataques e habilidades de combate são bloqueados na Cidadela segura, com aviso ao jogador; funcionam no Bosque. O jogador também usa C para interagir, I para a bolsa, J para o diário e Esc para pausa/opções. F6 e F8 pertencem à cena técnica histórica, não ao loop da campanha.

## 5. Narrativa que deve permanecer estável

Bram é ferreiro; Lyra, alquimista; Irian, líder do conselho; Maera, guarda; Sera, mercadora. A Caça ao Espião preserva local, duas decisões e identificadores existentes. O `quest_tracker` é estado de domínio, não uma cópia divergente de variáveis do diálogo.

A Cidadela pode mudar visualmente e em suas falas, mas mantém segurança e serviços essenciais. Morte, retry e respec não apagam escolhas. Revelações de Vael e finais ficam nos documentos com spoilers, sem necessidade de expor o final na apresentação do protótipo.

## 6. Estado real no início deste marco

| Componente | Evidência existente | Trabalho restante |
|---|---|---|
| Save | JSON, SHA-256, backup e testes do núcleo | Estado expandido validado, migração, inventário/recompensas e checkpoint |
| Ator/IA | ActorBase e EnemyFSM com testes | Conteúdo/variações dos inimigos, navegação e boss |
| Player | Movimento 2:1, dash, i-frames, custos e morte | Habilidades/guarda e integração com recuperação |
| Ataques | Básico/pesado, MP, direção fixa, registro de alvos e paredes | Balanceamento, interrupções, esquiva do boss e colisão de movimento rápido |
| Apresentação | Cena geométrica, HUD e efeitos opcionais | Tiles, sprites, animações, áudio e UI final |
| Mundo/quest | Documentação e narrativa | Mapas jogáveis, interações e consequências no runtime |

O PR #17 registrou 125 verificações headless no Godot 4.6. Isso valida os comportamentos cobertos, não a campanha, desempenho, acessibilidade completa ou export Windows. Os PRs #1–#18 foram integrados à main em ordem de dependência em 2026-10-13, após nova execução das 125 verificações. Veja o estado consolidado em [versões](versions.md).

## 7. Arquitetura e fronteiras

- **Domínio:** cálculos de combate e regras de quest, inventário, recompensa e progresso. Sem dependência de UI ou plugin de diálogo.
- **Controladores:** aceitam comandos, confirmam recursos/estado e impedem repetição. Comandos recusados não iniciam ação nem alteram recursos.
- **Adaptadores Godot:** Player/Enemy, física, input, cena e persistência. Matemática de alcance usa a métrica lógica; apresentação projeta o eixo vertical.
- **Conteúdo:** parâmetros em Resources confiáveis do projeto, IDs estáveis e catálogo validado. Saves externos não são scripts nem Resources executáveis.
- **Apresentação:** HUD, animações, áudio e efeitos recebem fatos confirmados; nunca concedem dano, recompensa ou progressão.
- **Persistência:** candidato completo validado antes de confirmar inventário, XP, flags e loot pendente; envelope/hash e migrações preservam compatibilidade.

Eventos notificam fatos. Falhas de gravação não podem ser convertidas em sucesso visual. A autoridade é local no offline; SHA-256 detecta corrupção, não comprova vitória nem impede edição deliberada. Conta, nuvem e telemetria só entram com contratos próprios na Alpha.

## 8. Uso das referências fornecidas

| Projeto | Aproveitamento escolhido | Fronteira |
|---|---|---|
| Godot Tactical RPG | Composição ator/modelo/serviço, já refletida no combate | Preservar tempo real 2D; não transportar sistema de turnos |
| Maaack Game Template | Candidato para menus, opções e carregamento de cenas | Integrar por necessidade e versão fixa; preservar save JSON próprio |
| Dialogic | Candidato para criação/apresentação de diálogos | Adaptador informa escolhas ao domínio; plugin não controla economia ou save |

Não instalar os três projetos completos como fundações concorrentes. Ao incorporar arquivo externo, registrar origem, versão e licença específica. As leituras e arquivos consultados estão em [combate offline](offline-combat.md).

## 9. Fora da primeira fatia

Multiplayer/PvP, ranking/economia compartilhada, autenticação obrigatória, backend de combate, versões mobile/web, outras quatro classes jogáveis, atos II–IV, geração procedural, crafting avançado, monetização, endgame e modo Eclipse. A arquitetura deve permitir evolução, sem implementar infraestrutura hipotética antes do loop offline.

## 10. Marcos e aceite

1. **Base de combate:** controles aprovados, confirmação única, defesa/habilidades, prioridades de interrupção e testes de fronteira. Aceite: arena mínima permite lutar, gastar recursos, esquivar e morrer corretamente.
2. **Mundo e recuperação:** Cidadela/Bosque conectados, inimigos e checkpoint. Aceite: morrer e retornar sem corromper progresso ou ficar preso.
3. **Recompensas e quest:** inventário, loot/XP atômicos e as duas rotas do espião. Aceite: concluir ambas em campanhas separadas, salvar/reabrir e observar consequência sem duplicação.
4. **Apresentação e distribuição:** arte/áudio mínimos, menus e export Windows. Aceite: playtest fora do editor, incluindo efeitos reduzidos e controles mostrados corretamente.

Sem datas artificiais: estimar prazo após o primeiro playtest e inventário dos assets. Meta inicial de sessão proposta: 15–25 minutos para percorrer o loop; validar em playtest antes de tratar como compromisso.

## 11. Riscos e decisões pendentes

Colisão atual é amostragem discreta, não sweep histórico. A onda precisa de simulação e playtest; conta analítica isolada não fixa uma janela justa. Limites de inventário, loadout final, atributos da arma, recursos da guarda e regras de regeneração precisam ser reconciliados em catálogo. Arte gerada é referência visual, não spritesheet operacional. Licenças, qualidade do export e perfil no hardware-alvo são gates de entrega.

O próximo resultado demonstrável é uma arena com habilidades/defesa e retry, seguida pelo percurso Cidadela → Bosque → Cidadela. A proposta visual está em [proposta visual](../design/visual-proposal.md).
