# 🌑 Echoes of Aethel

> **O caminho para o inferno está cheio de boas intenções.**
> Em Aethel, salvar alguém pode ser a primeira etapa de uma tragédia.

**Dark Fantasy ARPG 2D · Pixel Art 16-bit · Combate em tempo real**

Um RPG de ação sobre herança, culpa e poder em um mundo de magia decadente. A apresentação busca a expressividade dos RPGs clássicos como **Final Fantasy VI**; combate, exploração e progressão de equipamentos recebem a influência de **Diablo**. Personagens, arte, música e universo terão identidade própria.

Cinco heróis atravessam florestas contaminadas, cidades sepultadas e máquinas ancestrais. Entre uma batalha e outra, decisões tomadas para proteger os vivos alimentam uma força que transforma o mundo por dentro.

> **Estado atual:** documentação de design/arquitetura e implementação inicial de persistência offline. Há cena técnica Godot com Player/dash, parede, inimigo e HUD, com 84 verificações aprovadas; ainda não há campanha jogável. Habilidades, fórmulas e taxas de loot são especificações experimentais, sujeitas a implementação e playtest.

## 📜 História — O Coração partido

### O prólogo

Cinco Santuários filtravam a magia selvagem do **Coração de Éter** e sustentavam o equilíbrio de Aethel. Durante um eclipse, **Vael**, o Supremo Arquimago, tentou perfurar esse núcleo para extrair poder ilimitado.

O Coração se partiu em cinco fragmentos. A **Bruma Estelar** espalhou-se pelas terras: animais perderam sua natureza, mortos voltaram a caminhar e antigos guardiões se tornaram ameaças. Na **Cidadela dos Últimos**, um fragmento ainda puro mantém uma pequena região protegida.

### A jornada e o mistério de Vael

O herói chega à Cidadela por uma razão pessoal: redenção, cura, dever, sobrevivência ou ruptura com uma fé corrompida. Para alcançar o Pináculo Celestial, precisa enfrentar os guardiões e recuperar os três fragmentos sob seu domínio. O quinto permanece com Vael.

Ao longo da jornada, criaturas reconhecem uma marca que o protagonista não compreende. Registros antigos contradizem o que ele sabe sobre sua origem. No Mapa 12, o confronto com **Vael, o Sol-Negro**, revela por que seu caminho e o ritual parecem inseparáveis.

### Boas intenções, consequências sombrias

Cada dilema narrativo oferece duas alternativas. Ambas podem parecer necessárias; ambas preservam alguma coisa e condenam outra. Misericórdia e julgamento canalizam a energia do mundo para a mesma engrenagem ritualística, mas mudam pessoas, encontros e destinos de maneiras distintas.

As consequências exatas permanecem ocultas no momento da escolha. Sintomas, relatos e inscrições oferecem pistas; o jogador observa os efeitos depois, nos mapas e no hub. A tragédia não apaga sua influência: suas decisões determinam quem sobrevive, quem sofre e o que restará no epílogo.

## ⚔️ Mecânicas principais

### Doze mapas, quatro atos e um lugar para voltar

A campanha contém **12 mapas em 4 atos**, com exploração, encontros e arenas. A Cidadela é o hub principal; acampamentos seguros apoiam os atos posteriores. Cura, inventário e, na visão completa, ferreiro, alquimista, encantador e baú permitem preparar a próxima expedição.

Com o avanço da história e das escolhas, o hub se deteriora: moradores adoecem, crenças endurecem e a escassez muda diálogos e cenários. **Sua segurança mecânica permanece:** não há inimigos ativos nem dano no hub, e serviços essenciais continuam disponíveis.

### Quests de dupla escolha

Dilemas morais com duas opções, pistas prévias e consequências imediatas ou atrasadas. Decisões persistem no save e não são desfeitas por morte, respawn ou respec. Menus e interações operacionais não seguem a regra de duas escolhas.

### Combate, experiência e loot

Ataques em tempo real combinam posicionamento, esquiva, gestão de MP, telegraphs e habilidades de classe. Cada classe possui quatro habilidades ativas e duas passivas no módulo inicial de design.

| Encontro | Progressão e recompensa previstas |
|---|---|
| Inimigos comuns | XP menor; chance de ouro, consumíveis e equipamentos comuns ou mágicos |
| Elites | XP maior e alta chance de mágicos/raros; materiais de crafting |
| Sub-bosses | Equipamento raro ou épico garantido e essências elementais |
| Bosses de ato | Equipamento épico ou lendário; receita e fragmento únicos na primeira vitória da campanha |

Raridades: **Comum → Mágico → Raro → Épico → Lendário**. Nível do item e afixos determinam seu poder junto da raridade. Novas expedições podem repovoar encontros; recompensas únicas e decisões de história não se repetem.

### Dificuldade e recursos

Sem seletor tradicional: a progressão por atos e a Bruma mudam o desafio de novas expedições em mapas revisitados. HP/MP e stamina são confirmados pelo domínio; fúria do Bastião será buff passivo; poções podem ser compradas no hub e usadas por atalhos. O modo Eclipse, planejado para após a primeira conclusão, cria uma nova campanha com morte permanente e fases adicionais de boss; ainda não está implementado.

### Arte e interface 16-bit

Perspectiva **top-down 3/4 com projeção simulada 2:1**, geometria e colisões a validar, tiles de 16×16 e sprites jogáveis de **32×32 pixels**. Bosses ocupam células maiores, com névoa, raízes e luz de Éter compondo a atmosfera sem esconder sinais de ataque.

Caixas de diálogo azul-escuras usam **borda dupla de prata e marfim**, fontes pixel art legíveis e retratos expressivos de 64×64. Texto ampliável, avanço manual e efeitos reduzidos fazem parte da direção de interface. Cor, forma e som trabalham juntos na leitura do combate.

## 🧙 Cinco classes jogáveis

Os nomes clássicos descrevem o papel; os títulos pertencem ao universo de Aethel. Os atributos distribuíveis são **STR, DEX, INT, FTH e VIT**.

| Classe / título | Papel de combate | Atributo principal | Sprite 32×32 | Origem | Habilidade assinatura |
|---|---|---|---|---|---|
| **Mago — Tecelão de Runas** | Magia de área, controle e barreiras | INT — Inteligência | Manto violeta, runas claras, cajado e sombras azuis | Sobreviveu à ordem que rompeu os nós da realidade; busca selar as fendas e reparar seus erros | **Nó de Éter:** cria uma barreira temporária |
| **Ladina — Sentinela das Sombras** | Adagas, armadilhas e reposicionamento | DEX — Destreza | Capa azul-noite, prata e detalhes vermelhos | Uma execução ativou um selo; sua velocidade extraordinária vem de uma marca que consome vitalidade | **Passo Velado:** ocultação breve para preparar o próximo golpe |
| **Guerreiro — Bastião do Crepúsculo** | Defesa frontal e ataques pesados | STR — Força | Armadura azul-ferro, escudo âmbar e machado | Fechou os portões para salvar a cidade, abandonando família e soldados do lado de fora | **Guarda do Crepúsculo:** reduz dano frontal por tempo limitado |
| **Druida — Invocadora de Espinhos** | Invocações, veneno e sustentação | INT — Inteligência | Musgo, osso e violeta; cajado ramificado | Sua tribo fundiu as almas à natureza; as vozes ancestrais permanecem em seu cajado | **Golem de Pedra:** invoca um protetor temporário |
| **Paladino — Inquisidor da Chama Sagrada** | Maça, dano sagrado e autocura | FTH — Fé | Marfim, cobre e chama azul; tabardo gasto | Renegou a seita ao descobrir a aliança de seu líder com as forças da corrupção | **Veredito Solar:** projeta um golpe sagrado em cone |

Custos, cooldowns, passivas, atributos secundários e reações a Vael estão no [Módulo de Classes e Atributos](docs/GDD/01_classes_e_atributos.md).

## 🗺️ Atos e bosses

| Ato | Mapas | Região e progressão | Confronto principal |
|---|---|---|---|
| **I — O Despertar das Sombras** | 1–3 | Bosque das Névoas → Pântano dos Lamentantes → Portal do Julgamento. Garras de Musgo e Matriarca das Águas antecedem o guardião | **Silvano, o Traidor**, espírito da floresta manifestado em armadura antiga |
| **II — As Cinzas do Império** | 4–6 | Dunas de Sal → Vale dos Sacrifícios → Cripta de Ossos. Autômatos, altares e mortos cristalizados expõem o preço da antiga prosperidade | **Malignus, o Colecionador de Ossos** |
| **III — O Abismo das Máquinas e da Carne** | 7–9 | Esgotos de Éter → Fundição Proibida → Abismo da Voracidade. A Mãe das Proles e o Autômato Forjador guardam a descida | **Gorgoroth, a Boca das Sombras** |
| **IV — O Pináculo Celestial** | 10–12 | Jardins Suspensos → Ponte dos Lamentos → Coração de Éter. Arconte de Prata e Guardião dos Portões protegem o caminho final | **Vael, o Arconte Primogênito / Sol-Negro** |

No **Mapa 12**, a luta e a revelação convergem. Após derrotar Vael, o jogador decide entre restaurar a Ordem ou assumir a Ascensão. Nenhuma rota oferece salvação sem custo; decisões anteriores moldam os epílogos.

> Os documentos de campanha contêm spoilers completos. A apresentação acima preserva o segredo da origem do protagonista.

## 📚 Índice do GDD

| Documento | Conteúdo / estado |
|---|---|
| [Índice geral — docs/GDD/](docs/GDD/README.md) | Organização e referências canônicas do projeto |
| [01 — Classes e Atributos](docs/GDD/01_classes_e_atributos.md) | Sprint 1 documentada: fórmulas, cinco classes, habilidades e diálogos |
| [02 — Arquitetura Técnica](docs/GDD/02_arquitetura_tecnica.md) | API, PostgreSQL, paperdoll e deploy; cliente revisado na Sprint 6 |
| [03 — Sistema de Loot](docs/GDD/03_sistema_de_loot.md) | Raridades, afixos, slots, drops, consumíveis e JSON de itens |
| [04 — Mapas e Quests — spoilers](docs/GDD/04_mapas_e_quests.md) | Doze mapas, 24 opções e degradação narrativa da Cidadela |
| [05 — Combate e HUD](docs/GDD/05_combate_e_hud.md) | Fórmulas, HP/MP/vigor, Bruma, Eclipse e feedback visual |
| [06 — Arquitetura de Software](docs/GDD/06_arquitetura_software.md) | Godot/GDScript, componentes, eventos, entidades e diálogo |
| [07 — Roteiro e Diálogos — spoilers](docs/GDD/07_roteiro_e_dialogos.md) | Prólogo, NPCs dinâmicos, confronto com Vael e dois finais |
| [08 — Diálogos e Degradação do Hub — spoilers](docs/GDD/08_dialogos_hub_degradacao.md) | Bram, Lyra, Ancião/refugiados e seleção de diálogo Godot |
| [Sprint 8 — Save System](docs/GDD/08_save_system.md) | Persistência JSON, backup/recuperação e testes Godot |
| [09 — Enemy FSM](docs/GDD/09_enemy_fsm.md) | Estados de IA, percepção, ataques e NPCs hostis offline |

O [índice geral](docs/GDD/README.md) reúne também os documentos existentes de campanha, arte, loot, prólogo e engenharia. As próximas Sprints ampliarão os módulos sem duplicar o cânone.

## 🛠️ Desenvolvimento

[Apresentação Base/Melhorada](docs/engineering/enhanced-presentation.md): F8 alterna e F9 reduz efeitos.

[Executar a cena técnica do Player](docs/engineering/player-test-scene.md): abrir client/godot/project.godot no Godot 4.6 e pressionar F5.

As [decisões de produção aprovadas](docs/engineering/production-decisions.md) prevalecem sobre propostas anteriores: Windows offline, slice Bastião/Cidadela/Bosque, MP em ativas e stamina para dash. Runtime ainda precisa dessa reconciliação.

**Primeiro marco jogável:** Cidadela → Bosque das Névoas → retorno à Cidadela, com Bastião, combate, Garras de Musgo, loot e uma escolha persistente com consequência observável.

A primeira entrega será **single-player offline**. A Sprint 6 define Godot/GDScript para o cliente, substituindo a proposta Flutter/Flame, e mantém Python/FastAPI/PostgreSQL para autoridade online. Windows é a plataforma inicial. Alpha prevê conta/saves em nuvem/telemetria, sem combate autoritativo no lançamento single-player; instâncias competitivas exigirão contrato próprio. Domínio, conteúdo, apresentação e persistência terão limites claros; saves serão versionados e efeitos/recompensas protegidos contra duplicação.

Cada entrega é publicada com commit e PR conforme o [fluxo do GitHub](docs/engineering/github-workflow.md). Este README apresenta a visão do jogo; o [roadmap](docs/engineering/roadmap.md) distingue planejamento de implementação.
