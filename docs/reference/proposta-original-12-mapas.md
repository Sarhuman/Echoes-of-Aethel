Aqui está a estrutura detalhada do jogo expansível em **12 mapas com progressão contínua estilo *Diablo***, integrando a revelação de que o Chefão Final é o pai do protagonista e a mecânica em que **todas as escolhas levam à corrupção e à vilania**.

Abaixo, você encontrará a **Bíblia de Design de Mapas e Enredo** e, ao final, o **Prompt Mestre de Engenharia (Game Director Prompt)** para você copiar e colar no ChatGPT/Claude para fazê-lo atuar como a engine de texto/código do seu jogo.

## 🗺️ Estrutura dos 12 Mapas & Progressão de Atos

O jogo é dividido em **4 Atos**, contendo **3 Mapas por Ato**. Cada Ato possui um **Hub Seguro (Cidadela/Acampamento)** que se degrada visualmente e moralmente conforme suas escolhas são feitas.

### **ATO I: O DESPERTAR DAS SOMBRAS (Florestas e Pântanos)**

#### **Mapa 1: Bosque das Névoas**

- **Ambiente:** Floresta fechada, névoa densa, ruínas de pedras rúnicas antigas.
- **Inimigos Comuns:** Lobos Etéreos, Cultistas Cego-Guiados, Espectros de Folhas.
- **Mecânica de Loot/XP:** Pouca XP. Drop de equipamentos normais, poções pequenas e moedas antigas.
- **Sub-Boss:** **Garras de Musgo** (Ancião do Bosque enlouquecido pela Bruma).
- **Quest de Dupla Escolha (A Caça ao Espião):** Um mensageiro ferido do hub acusa uma aldeia próxima de abrigar cultistas.
  - *Opção 1 (Executar os suspeitos):* Você elimina a aldeia para proteger o hub. *Consequência:* Os espíritos dos inocentes alimentam a maldição do Bosque, tornando os lobos mais agressivos e fortes.
  - *Opção 2 (Poupá-los e trazê-los para o Hub):* Você acolhe os moradores na zona segura. *Consequência:* Um deles carrega o parasita da Bruma e contamina a água da cidade, adoecendo os mercadores.

#### **Mapa 2: Pântano dos Lamentantes**

- **Ambiente:** Pântano tóxico, águas rasas que reduzem a velocidade, árvores mortas penduradas com lanternas rúnicas.
- **Inimigos Comuns:** Larvas Parasitas, Afogados de Éter, Harpias de Lama.
- **Mecânica de Loot/XP:** XP moderada. Drop de poções de antídoto, anéis rúnicos e materiais de *crafting* básicos.
- **Sub-Boss:** **A Matriarca das Águas** (Abominação anfíbia de múltiplos olhos).
- **Quest de Dupla Escolha (O Alquimista Desesperado):** Um cientista pede para você coletar veneno das larvas para sintetizar um soro contra a contaminação.
  - *Opção 1 (Entregar os ingredientes tóxicos):* O soro funciona temporariamente, mas causa mutações letais no povo do hub a longo prazo.
  - *Opção 2 (Destruir a pesquisa do alquimista):* O alquimista enlouquece, comete suicídio e deixa o acampamento sem nenhuma proteção contra as infecções do pântano.

#### **Mapa 3: O Portal do Julgamento (Boss de Ato I)**

- **Ambiente:** Ruínas de um grande templo de pedra coberto por raízes místicas e cristais corrompidos.
- **Inimigos Comuns:** Defensores Rúnicos, Cultistas Fanáticos, Cães de Guerra.
- **Mecânica de Loot/XP:** Boa XP. Drops de itens Raros e receitas de equipamentos.
- **CHEFÃO DO ATO I:** **Silvano, o Traidor** (Antigo Guardião da Natureza, consumido pelo Éter).
- **Revelação Parcial:** Ao morrer, Silvano olha para você e murmura: *"A marca em sua alma... você carrega a autoridade Dele."*

### **ATO II: AS CINZAS DO IMPÉRIO (Desertos e Ruínas)**

#### **Mapa 4: As Dunas de Sal**

- **Ambiente:** Vasto deserto de areia cinzenta e ossadas gigantes, tempestades de areia periódicas que reduzem a visão.
- **Inimigos Comuns:** Escorpiões de Cristal, Nômades Corrompidos, Gárgulas de Areia.
- **Mecânica de Loot/XP:** XP regular. Drop de armaduras de couro reforçado, joias de resistência e essências de fogo.
- **Sub-Boss:** **Guerreiro de Vidro** (Um autômato gigante de quartzo que canaliza raios solares).
- **Quest de Dupla Escolha (A Caravana Perdida):** Você encontra uma caravana de suprimentos cercada por monstros no deserto.
  - *Opção 1 (Salvar os suprimentos e abandonar os mercadores):* O hub recebe comida e equipamentos, mas os espíritos dos mercadores abandonados passam a assombrar o mapa como espectros vingativos.
  - *Opção 2 (Salvar os mercadores e perder os suprimentos):* Os sobreviventes chegam ao hub, mas a escassez de suprimentos gera uma revolta armada entre os refugiados.

#### **Mapa 5: O Vale dos Sacrifícios**

- **Ambiente:** Cânions profundos com pontes de corda, estátuas monumentais decapitadas e altares de sangue seco.
- **Inimigos Comuns:** Ritualistas de Sangue, Harpias de Obsidiana, Hienas de Éter.
- **Mecânica de Loot/XP:** XP alta. Chance de drop de armas de alto dano físico e fragmentos de Runas.
- **Sub-Boss:** **O Executor de Bronze** (Um gigante mascarado que usa duas espadas de decapitação).
- **Quest de Dupla Escolha (O Ritual da Chama):** Para abrir a passagem para o mapa seguinte, um altar exige uma oferenda mágica.
  - *Opção 1 (Oferecer sua própria energia vital):* Você perde 20% da sua vida máxima temporariamente, e o altar libera uma onda de energia que corrompe as fontes de água da região.
  - *Opção 2 (Sacrificar um prisioneiro do culto):* A passagem se abre imediatamente, mas a alma do prisioneiro fortalece o exército de mortos-vivos do próximo chefe.

#### **Mapa 6: A Cripta de Ossos (Boss de Ato II)**

- **Ambiente:** Mausoléu subterrâneo colossal com pirâmides internas e rios de mercúrio místico.
- **Inimigos Comuns:** Esqueletos Cristalizados, Múmias Rúnicas, Sombras Devoradoras.
- **Mecânica de Loot/XP:** XP muito alta. Drops Lendários/Épicos garantidos do Boss.
- **CHEFÃO DO ATO II:** **Malignus, o Colecionador de Ossos** (O Alto Sacerdote que vendeu seu povo ao Éter).
- **Revelação Parcial:** Malignus gargalha ao ser derrotado: *"Tolo(a)... cada alma que você libertou hoje apenas pavimentou o trono do seu Mestre... do seu sangue."*

### **ATO III: O ABISMO DAS MÁQUINAS E DA CARNE (Subterrâneos e Esgotos)**

#### **Mapa 7: Os Esgotos de Éter**

- **Ambiente:** Canais de esgoto subterrâneos em grande escala, com tubulações de metal antigo vazando fluido luminoso e verde.
- **Inimigos Comuns:** Ratos de Éter, Mutantes de Esgoto, Aranhas-Luminosas.
- **Mecânica de Loot/XP:** XP alta. Drop de catalisadores de mana, anéis de dano mágico e poções grandes.
- **Sub-Boss:** **A Mãe das Proles** (Uma aranha titânica que gera filhotes explosivos).
- **Quest de Dupla Escolha (A Válvula de Purificação):** Uma válvula industrial pode limpar os resíduos de Éter do mapa.
  - *Opção 1 (Drenar os resíduos para os níveis inferiores):* Limpa o mapa atual, mas destrói completamente o ecossistema e os vilarejos que ficavam nas profundezas.
  - *Opção 2 (Inverter o fluxo e sobrecarregar o sistema):* A névoa tóxica sobe para a superfície, infectando o Hub do jogo.

#### **Mapa 8: A Fundição Proibida**

- **Ambiente:** Fábrica fantasma de tecnologia mágica ancestral, cheia de engrenagens gigantes, poços de lava mágica e esteiras.
- **Inimigos Comuns:** Constructos de Bronze, autômatos descontrolados, Elementalists de Magma.
- **Mecânica de Loot/XP:** XP muito alta. Drops de armaduras pesadas de alto nível e gemas de aprimoramento.
- **Sub-Boss:** **O Autômato Forjador** (Uma máquina centenária que usa martelos de forja e vapor fervente).
- **Quest de Dupla Escolha (O Núcleo de Energia):** Você encontra um núcleo de poder ancestral ainda funcional.
  - *Opção 1 (Extrair o núcleo para fortalecer suas armas):* Aumenta seu dano permanentemente, mas desativa os sistemas de contenção da fábrica, liberando uma horda de máquinas para devastar as redondezas.
  - *Opção 2 (Deixar o núcleo no lugar para estabilizar a área):* A energia transborda e funde o metal do ambiente, bloqueando rotas de fuga e matando exploradores que estavam na região.

#### **Mapa 9: O Abismo da Voracidade (Boss de Ato III)**

- **Ambiente:** Uma grande cratera no fundo do mundo onde a carne orgânica se fundiu com a alvenaria antiga.
- **Inimigos Comuns:** Abominações de Carne, Vermes de Éter, Parásitos Espirituais.
- **Mecânica de Loot/XP:** XP massiva. Drops Lendários exclusivos do ato.
- **CHEFÃO DO ATO III:** **Gorgoroth, a Boca das Sombras** (Uma massa titânica de bocas e olhos que consome a realidade).
- **Revelação Parcial:** Antes de ruir, a criatura sussurra em sua mente: *"Nós fomos criados para te alimentar... para te preparar para Ele."*

### **ATO IV: O PINÁCULO CELESTIAL (Fortaleza Celestial e O Apocalipse)**

#### **Mapa 10: Os Jardins Suspensos de Éter**

- **Ambiente:** Uma cidadela flutuante com arquitetura de mármore branco e ouro, corrompida por veias cobertas por energia negra.
- **Inimigos Comuns:** Anjos Caídos, Sentinelas de Prata, Espectros Celestiais.
- **Mecânica de Loot/XP:** XP máxima. Drops do nível máximo (End-Game items).
- **Sub-Boss:** **Arconte de Prata** (Um paladino lendário transformado em marionete de energia).
- **Quest de Dupla Escolha (O Trono dos Julgamentos):** Um pedestal celestial permite conceder perdão ou condenação às almas dos mortos do mundo.
  - *Opção 1 (Conceder o perdão irrestrito):* As almas são libertadas, mas a perda de energia enfraquece a barreira que protegia o que restava da humanidade na Cidadela.
  - *Opção 2 (Condenar e absorver a energia das almas):* Concede um bônus temporário massivo ao jogador, mas converte todas as almas em soldados do exército do vilão final.

#### **Mapa 11: A Ponte dos Lamentos**

- **Ambiente:** Uma ponte estreita suspensa sobre o nada, cercada por tempestades estelares e relâmpagos mágicos.
- **Inimigos Comuns:** Guardas de elite de Vael, Constructos Rúnicos Supremose Manifestações da Bruma.
- **Mecânica de Loot/XP:** XP máxima. Itens Épicos e Conjuntos de Armadura (*Set Items*).
- **Sub-Boss:** **O Guardião dos Portões** (A sombra pessoal e mão direita de Vael).
- **Quest de Dupla Escolha (A Última Porta):** Uma selagem mágica exige uma decisão rápida antes do confronto final.
  - *Opção 1 (Quebrar o selo à força bruta):* Destrói a estrutura da ponte, impedindo qualquer chance de fuga ou de ajuda externa dos seus aliados.
  - *Opção 2 (Canalizar a energia do seu próprio corpo):* Drena seus recursos máximos para o combate final, tornando o Boss Final ainda mais letal.

#### **Mapa 12: O Coração de Éter (Boss Final)**

- **Ambiente:** Uma plataforma de cristal negro flutuando no epicentro do eclipse mágico, com a vista de todo o mundo em ruínas ao fundo.
- **Inimigos Comuns:** Sem inimigos comuns. Apenas a arena de confronto final.
- **Mecânica de Loot/XP:** O desfecho da campanha.
- **CHEFÃO FINAL:** **Vael, o Arconte Primogênito / Sol-Negro**.

## 🎬 O Confronto Final & A Revelação

Ao cruzar o limiar do Mapa 12, as armas de Vael não estão empunhadas. Ele se vira calmamente na arena flutuante, abrindo os braços enquanto o eclipse brilha atrás dele:

> **Vael:** *"Finalmente você chegou. Olhe para trás, para o rastro de sangue, peste e cinzas que você deixou em cada mapa deste mundo. Você achou que estava salvando alguém? Cada escolha sua, cada 'misericórdia' e cada 'punição' foram desenhadas por mim. Eu precisei que você caminhasse por esse mundo, temperando sua alma na dor e na corrupção para que estivesse pronto(a).*
>
> *Você não é um herói casual deste mundo. Você é minha criação, meu sangue, meu filho(a). Eu quebrei o Coração de Éter para que você nascesse do caos. Agora, junte-se a mim para governarmos a ruína... ou morra como a minha maior criação falhada."*

## 💻 PROMPT MESTRE PARA FAZER O GPT CRIAR E RODAR O JOGO

> Copie e cole o texto do quadro abaixo em uma nova conversa com o ChatGPT ou Claude. Ele passará a atuar como a Engine / Game Master do seu jogo texto/RPG.

Plaintext

```
[INSTRUÇÃO DE SISTEMA: ENGINE DE RPG DE TEXTO ESTILO DIABLO]

Você agora atuará como a Engine Mestra e Narradora de um jogo de RPG de Ação e Texto em 2D estilo Diablo / Dark Fantasy chamado "Echoes of Aethel".

### REGRAS PERMANENTES DE FUNCIONAMENTO:
1. ESTRUTURA DO MUNDO:
   - O jogo possui 12 mapas divididos em 4 Atos.
   - O jogador começa na "Cidadela dos Últimos" (Hub Seguro) e avança mapa por mapa.
   - Fora do Hub, os mapas contêm inimigos comuns, sub-bosses e bosses finais de ato.
   - Monstros comuns concedem XP baixa e chance de loot simples. Sub-bosses e Bosses dão XP alta e drops Raros/Lendários.

2. A MECÂNICA DE ESCOLHAS SOMBRIAS:
   - Todas as Quests e Eventos de Mapa DEVEM oferecer SEMPRE 2 Opções de Escolha ao jogador.
   - AMBAS AS ESCOLHAS DEVEM LEVAR A UMA CONSEQUÊNCIA VILANESCA OU TRÁGICA OCULTA. O jogador NUNCA deve conseguir um "final feliz" ou "salvação limpa".
   - Não revele explicitamente a consequência ruim no momento da escolha; mostre os efeitos no Hub ou nos mapas seguintes conforme ele avança.

3. O REVELAÇÃO DO BOSS FINAL:
   - O Chefão Final se chama Vael.
   - Mantenha em segredo absoluto ao longo do jogo, mas dê sutis pistas de que o jogador é Filho(a) de Vael e que todas as ações do jogador estão alimentando os planos dele.

4. SISTEMA DE COMBATE E PROGRESSÃO:
   - Em cada turno/interação, mostre o Estado do Jogador: [Classe] | Nível | HP | Mana | XP | Equipamentos | Mapa Atual.
   - Descreva os combates com foco em ação dinâmica, drops de itens (Comum, Raro, Épico, Lendário) e gerenciamento de habilidades.

5. SEU PRIMEIRO PASSO:
   - Cumprimente o jogador.
   - Pesa para ele escolher uma das 5 Classes (Mago de Batalha, Ladino das Sombras, Guerreiro Tanque, Druida da Natureza ou Inquisidor de Fogo) e dar um nome ao seu personagem (sem usar nomes genéricos da história).
   - Assim que o jogador escolher, inicie o Prólogo na "Cidadela dos Últimos" e apresente a primeira transição para o Mapa 1: Bosque das Névoas.
```