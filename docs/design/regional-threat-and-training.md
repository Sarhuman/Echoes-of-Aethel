# Nível regional, leitura de ameaça e formação nos hubs

**Decisão de direção — 16/10/2026:** o proprietário delegou as definições de progressão/especialização e pediu clareza sobre níveis, cores e mentores em outras regiões. As regras abaixo são design para o cliente Unity. Faixas, marcos e potências novos são baseline **EXPERIMENTAL**, sujeitos a testes; não descrevem inimigos ou UI já implementados. Não reconfiguram o cliente Godot.

## Um mundo com regiões de dificuldade própria

As quatro bases de retorno continuam seguras: Cidadela, Acampamento das Dunas, Refúgio Subterrâneo e Posto do Limiar. Não existem monstros hostis dentro desses hubs. “Nível da cidade” significa a dificuldade das rotas externas, não NPCs ou serviços que causam dano.

Cada encontro recebe nível do catálogo da sua região, e não do nível atual do herói. Subir um nível não fortalece simultaneamente o mesmo inimigo. Voltar a uma região anterior permite perceber a evolução. As rotas mantêm seus pré-requisitos canônicos; não adicionar um bloqueio de nível obrigatório. Entrar abaixo da recomendação é uma decisão informada quando a rota já estiver liberada.

| Mapa / ID canônico | Faixa experimental dos comuns | Encontro principal: nível experimental |
|---|---:|---:|
| Bosque / `forest_01` | 1–4 | Garras de Musgo: 4 |
| Pântano / `swamp_02` | 4–6 | Matriarca das Águas: 6 |
| Portal / `temple_03` | 6–8 | Silvano: 8 |
| Dunas / `dunes_04` | 8–10 | Guerreiro de Vidro: 10 |
| Vale / `valley_05` | 10–12 | Executor de Bronze: 12 |
| Cripta / `crypt_06` | 12–15 | Malignus: 15 |
| Esgotos / `sewers_07` | 15–17 | Mãe das Proles: 17 |
| Fundição / `foundry_08` | 17–20 | Autômato Forjador: 20 |
| Abismo / `abyss_09` | 20–22 | Gorgoroth: 22 |
| Jardins / `gardens_10` | 22–25 | Arconte de Prata: 25 |
| Ponte / `bridge_11` | 25–28 | Guardião dos Portões: 28 |
| Coração / `heart_12` | Sem comuns | Vael: 30 |

Esses números são níveis de inimigo NOVOS, não conversão das faixas de **nível de item** do GDD 04. Não aplicar automaticamente as fórmulas de atributos do herói a monstros. HP, defesa, dano, XP e distribuição de grupos precisam de catálogo e simulação próprios. Coroa de Sucata: comuns 10–12 e Gorvak 12; Livro dos Nomes Perdidos: comuns 14–16 e Namar 16, como dungeons opcionais mais exigentes, sem escalar com o jogador.

Nível do encontro é fixado na criação e preservado durante sua tentativa. Elite continua da mesma faixa, identificado por categoria/afixos; não precisa ganhar níveis para receber suas propriedades. Promoção em expedição revisitada e corrupção seguem as decisões existentes, sem empilhamento e sem mudar um combate em andamento. Não adicionar autoscaling, penalidade secreta por diferença de nível, nova chance de MISS ou multiplicador econômico nesta ficha. Uma mudança futura desses sistemas exige regra explícita.

## Número, nome e cor juntos

O alvo em foco mostra, por exemplo, “Lobo Etéreo · Nv. 4”. A cor informa comparação de nível; ela não muda dano, IA ou loot. Usar diferença `nível do inimigo − nível do herói`:

| Diferença | Cor semântica | Rótulo acessível |
|---|---|---|
| +4 ou mais | Vermelho escuro | Muito acima do seu nível; símbolo de ameaça |
| +1 a +3 | Vermelho claro | Acima do seu nível |
| 0 | Branco | Mesmo nível |
| −1 ou menos | Azul | Abaixo do seu nível |

Essa escala preserva a leitura pedida pelo proprietário. “Abaixo” não promete luta fácil: chefe/elite, afixos, composição do grupo e execução do jogador são informações independentes. Um boss abaixo do nível mantém símbolo e barra de boss; sua categoria não recolore o nome e não contradiz a comparação. Raridade de item usa seu próprio painel/paleta, nunca o nome do monstro como legenda de loot.

Mostrar número por padrão no alvo/boss; opção de nomes/barras persistentes para quem desejar. Evitar poluir a tela com todos os detalhes de todos os corpos. Não revelar inimigo oculto, atrás de parede ou fora da percepção só por ativar nomes. Atualizar comparação quando alvo ou nível mudar, reutilizando componentes de UI. Os limites de componentes visíveis são de desempenho a medir no PC real.

Vermelho escuro precisa de placa/contorno contrastante: não usar texto quase preto sobre cenário escuro. Versão acessível usa nível, rótulo e ícone além da cor. Permitir paleta alternativa sem alterar as faixas. Aviso da entrada de rota apresenta sua faixa recomendada antes da viagem, sem mostrar conteúdo narrativo oculto.

## Mentores e serviço regional

As mesmas pessoas não aparecem duplicadas em todas as regiões. Bram e os demais NPCs centrais mantêm sua identidade e localização narrativa; os hubs posteriores têm habitantes e responsáveis locais. Uma viagem real de NPC exige roteiro e estado de presença próprio, não instâncias clonadas com falas idênticas.

**Atribuição de serviço na Cidadela para este design:**

| Classe | Responsável existente | Escopo da conversa, sem inventar passado |
|---|---|---|
| Bastião | Bram | Preparação de armas, defesa e compromisso dos golpes |
| Tecelão | Irian | Acesso a registros e exercícios rúnicos; não pressupõe nova biografia de arquimago |
| Sentinela | Sera | Leitura de rotas, preparação e manuais de emboscada |
| Invocadora | Lyra | Pesquisa de vínculos/Éter e acesso a exercícios de controle |
| Inquisidor | Maera | Disciplina, proteção e acesso a registros de treinamento |

Esses responsáveis abrem o serviço e contextualizam a escolha; não são declarados mestres de todas as técnicas. O roteiro final deve usar as competências já existentes e materiais de formação. Atribuições são decisões de produção delegadas; falas, ilustrações e novos IDs ainda precisam ser produzidos.

Cada hub regional recebe um **responsável local de formação**, com pessoa/ID distintos e acesso aos mesmos registros de progresso por personagem. Seu papel é encaminhar a formação da classe e disponibilizar exercícios/manuais, não copiar a biografia do mentor da Cidadela. Para este incremento usar o papel editorial “responsável de formação” sem inventar nomes próprios persistidos. Fechar elenco e diálogo antes de anunciar esse serviço como implementado. O nome localizado exibido ao jogador nunca será chave de save.

O responsável regional pode abrir a primeira escolha ainda pendente, inclusive se o jogador já passou do nível 5. Não exigir voltar à Cidadela para recuperar um aviso perdido. O serviço de formação aparece na conversa normal de qualquer responsável compatível quando houver elegibilidade, mesmo sem popup.

**Recuperação de descoberta:** menu do personagem mostra estado bloqueado/disponível/escolhido; diário e marcador do hub apontam para o responsável mais próximo no hub atual, com a Cidadela como referência de retorno. Fora de hub, o diário aponta o próximo hub já desbloqueado ou a Cidadela. Não exibir nome de um mentor distante como se estivesse presente. Popup é lembrete, não gatilho obrigatório ou item de quest. Ao dispensar, o jogador mantém acesso; reload nunca apaga a elegibilidade.

Respec e confirmação só ocorrem em hub seguro, nunca ao encontrar um cidadão aleatório na rota. Se um personagem local ficar indisponível por narrativa, um substituto deve manter o serviço; não pode haver combinação de escolhas que remova a formação. A viagem de retorno e os hubs seguem contratos existentes de segurança e persistência.

## Aceite antes de integrar ao gameplay

- Cobrir limites de ameaça −1/0/+1/+3/+4, todos os níveis válidos até o limite experimental 30 e entradas inválidas; UI não muda estatísticas.
- Confirmar que elite/boss abaixo do herói conserva categoria/afixos e continua sem declaração enganosa de “seguro”.
- Subir nível ou entrar em hub não reescala encontros vivos; revisão de corrupção só ocorre no marco aprovado.
- Testar acesso à especialização sem popup, após dispensa, ganho múltiplo de níveis, reload e mudança de hub; mesma seleção em todos os responsáveis.
- Cada pessoa tem identidade única, localização coerente e nenhum clone narrativo. Instância indisponível possui substituto funcional.
- Simular a campanha com as cinco classes e equipamento comum, sem dungeons obrigatórias ou grind exigido por falta de XP. Ajustar números se o herói não chegar naturalmente ao nível 5 na janela inicial ou houver salto de dificuldade.

Faixas e critérios ficam versionados como proposta de balanceamento; testes de código, telas e playtests ainda precisam ser executados na implementação. Não declarar essas cidades, NPCs ou monstros criados por este documento.
