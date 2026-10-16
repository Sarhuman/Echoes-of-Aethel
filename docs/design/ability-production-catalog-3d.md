# Catálogo de produção das habilidades — cinco heróis

**Estado:** pré-produção 3D, sem ataques, animações ou efeitos Unreal conectados. A [Sprint 1](../GDD/01_classes_e_atributos.md) é a fonte mecânica: cada classe tem **A1–A4 (ativas) e P1–P2 (passivas)**, total de 30 nós. Esta página adiciona linguagem visual/sonora **NOVO/EXPERIMENTAL** para produzir assets e não altera P, custo, cooldown, duração ou pré-requisitos. Os [controles do slice](../engineering/decisions-first-slice.md) regem o mapeamento atual do Bastião; número A2/A3 na árvore não é a tecla da habilidade.

## Contrato compartilhado de leitura

- A1 é o ataque inicial gratuito de cada classe. A2 abre no nível 2, A3 no 4, A4 no 6; P1 após A2 no nível 3, P2 após A3 no 5. Compra de nó, pré-requisitos e custo de ponto vêm do GDD 01. Estar documentado não torna um nó disponível no cliente Unreal.
- Cada ativa precisa de **antecipação, execução, contato/área, recuperação e recusa** legíveis. O contato físico/efeito confirmado aciona dano/cura e feedback; brilho e som sozinhos não o fazem. Cooldown, MP insuficiente, alvo inválido ou animação bloqueada recusam sem gasto. Passivas têm indicador discreto de condição e disparo quando aplicável; não exigem botão.
- Pés e direção permanecem claros sob câmera elevada. Projéteis colidem com alvo/obstáculo; linhas/cones/áreas mostram limite no piso antes de resolver. Cor é apoio, nunca a única pista. Textos de dano, esquiva, imunidade e cura dependem do resultado confirmado. Efeitos reduzidos preservam silhueta, limite, tempo e som essencial.
- Ativas do Bastião usam **MP**, conforme D5; stamina é separada e paga a esquiva. Menções a “vigor” na tabela original são históricas para essa classe. As demais ativas já usam MP no GDD. Não criar uma terceira barra de vigor.
- O conteúdo de produção por habilidade só está completo com ID técnico aprovado, ícone, animações/rig, VFX/telegraph, SFX original, texto localizado, dados mecânicos versionados, colisão/alcance e variante de efeitos reduzidos. **IDs de habilidade ainda não estão estabilizados** nesta página; não derivá-los do nome exibido ou índice de tecla.

## Tecelão de Runas `rune_weaver`

| Nó e efeito canônico | Leitura 3D proposta | Som/risco de produção |
|---|---|---|
| A1 Projétil Rúnico — ataque arcano gratuito, primeiro alvo/obstáculo | Runa compacta deixa o cajado e conserva trajetória nítida até colisão | Pulso curto e impacto distinto em pedra/ator; evitar projétil que atravessa parede por VFX |
| A2 Lança de Geada — dano e lentidão menor em boss | Cristal fino com linha de viagem e breve marca de gelo nos pés do alvo | Estalo seco; estado lento visível sem cobrir telegraph do inimigo |
| A3 Círculo de Brasas — área e queimadura | Contorno no chão antes da ignição; brasas baixas durante os ticks | Crescimento e três pulsações de dano; não reaplicar ticks por reentrada visual |
| A4 Nó de Éter — barreira temporária | Polígono rúnico abraça o corpo, com capacidade/duração no HUD | Tom de ativação e ruptura; reaplicar substitui barreira, sem empilhar halos |
| P1 Fratura Rúnica — bônus ao próximo dano direto em alvo sob sua lentidão | Fissura breve na marca de gelo ao consumir | Um único sinal de consumo, sem duplicar impacto ou buff em DoT |
| P2 Memória do Selo — MP extra com barreira ativa fora de combate | Respiração lenta do nó enquanto regeneração é elegível | Som ambiente baixo; não sugerir regeneração durante combate |

## Sentinela das Sombras `shadow_sentinel`

| Nó e efeito canônico | Leitura 3D proposta | Som/risco de produção |
|---|---|---|
| A1 Corte de Adaga — golpe físico gratuito | Passo curto, braço e lâmina visíveis; contato único por alvo | Corte seco; duas adagas não autorizam dois acertos automáticos |
| A2 Passo Velado — ocultação breve, quebra ao atacar/sofrer dano | Silhueta permanece identificável ao jogador e sombra fica mais estreita | Abafamento breve; IA ainda conserva última posição conhecida |
| A3 Armadilha de Agulhas — uma ativa, expira, causa sangramento | Objeto baixo com contorno visível no piso e disparo direcional | Clique de armar/disparar; inimigos não podem usá-la para prender o herói contra parede |
| A4 Ruptura Tendínea — golpe e lentidão menor em boss | Avanço com arco baixo para distinguir da A1 | Impacto grave e marca de lentidão; não prometer stun de boss |
| P1 Última Sombra — bônus ao primeiro A1 após Passo Velado | Borda curta na lâmina até consumo ou expiração | Um sinal ao sair da ocultação e outro ao consumir; não repetir por segundo ataque |
| P2 Sangue Frio — dano direto contra alvo sob seu sangramento | Marca discreta de ferida ativa | Não amplificar ticks nem sinais de sangue em modo reduzido |

## Bastião do Crepúsculo `twilight_bastion`

| Nó e efeito canônico | Leitura 3D proposta | Som/risco de produção |
|---|---|---|
| A1 Golpe de Machado — ataque físico gratuito | Giro curto de ombro com arco frontal e pé de apoio fixo | Metal/madeira conforme alvo; hitbox não inclui ornamento do machado |
| A2 Guarda do Crepúsculo — redução frontal temporária | Escudo cobre o setor frontal; costas permanecem expostas | Som de levantar/receber golpe; direção de bloqueio deve ser verificável |
| A3 Impacto do Baluarte — dano e interrupção apenas em comuns | Passo pesado, impacto concentrado no piso/alvo; boss reage sem stun | Pancada curta e feedback de dano; não deixar faixa luminosa persistente após terminar |
| A4 Fenda de Aço — linha curta com antecipação | Linha no piso cresce durante 0,6 s e termina no alcance real | Ruptura grave; desenho não permanece após resolução nem alcança através de obstáculo |
| P1 Postura Firme — reforço temporário após primeiro bloqueio A2 | Escudo ganha segunda borda após bloqueio confirmado | Pulso único; respeitar teto total de redução do GDD |
| P2 Peso do Juramento — bônus de A4 com HP até metade | Marca discreta no ícone de Fenda quando elegível | Sem ruído contínuo ou incentivo visual a perder HP de propósito |

**Mapeamento atual do slice Godot:** `1` básico A1; `2/Q` Impacto A3; `3/E` Guarda A2; `4/R` Fenda A4; `5/F` reservado. A árvore A1–A4 e as teclas são conceitos distintos. O esquema definitivo das cinco classes na Unreal ainda requer design de input/HUD; não assumir que A2 sempre estará na tecla 2.

## Invocadora de Espinhos `thorn_summoner`

| Nó e efeito canônico | Leitura 3D proposta | Som/risco de produção |
|---|---|---|
| A1 Espinho Errante — projétil gratuito | Espinho claro e único cruza o espaço até contato | Assobio curto; colisão real define acerto |
| A2 Golem de Pedra — invocação limitada com corpo próprio | Pedra emerge em ponto navegável sem empurrar heroína ou inimigo contra parede | Peso no nascimento e passos próprios; colisor separado e limite de um golem |
| A3 Jardim de Pragas — área venenosa por três ticks | Raízes desenham borda no piso, vegetação baixa ocupa a área | Três pulsações suaves; reentrada não duplica o veneno |
| A4 Fera Ancestral — segunda invocação limitada | Silhueta animal distinta do golem, spawn em piso livre | Chamado e movimento próprios; limite de uma fera, reset limpa a entidade |
| P1 Raiz Mineral — DEF do golem | Veios minerais no golem, sem aura na heroína | Sinal ao invocar; não aumentar defesa da Invocadora |
| P2 Elo da Matilha — MP extra enquanto invocação viva fora de combate | Ligação tênue visível apenas em repouso | Sem ruído de loop forte; cessar ao morrer/substituir invocação |

## Inquisidor da Chama Sagrada `sacred_flame_inquisitor`

| Nó e efeito canônico | Leitura 3D proposta | Som/risco de produção |
|---|---|---|
| A1 Maça do Exílio — ataque físico gratuito | Peso da maça e contato curto, sem chama automática | Impacto de metal; não confundir com A2 sagrado |
| A2 Martelo de Cinzas — sagrado com queimadura | Fogo azul acompanha golpe e marca temporária do alvo | Chama abafada e ticks distintos; efeito não mascara ataque do boss |
| A3 Chama de Vigília — autocura, sem dano | Luz parte do peito/mão para o próprio corpo, nunca para o inimigo | Sopro ascendente; HP cheio recusa sem gastar MP |
| A4 Veredito Solar — cone frontal antecipado | Cone no piso durante 0,7 s, seguido por avanço de luz controlado | Crescimento musical curto; não usar flash que esconda bordas/aliados |
| P1 Brasa Persistente — um tick extra na A2 | Marca de chama prolongada uma vez | Não empilhar nova instância de queimadura |
| P2 Última Confissão — dano direto sagrado maior contra mortos-vivos | Marca contextual discreta no contato válido | Não afetar cura nem ticks; definir taxonomia de mortos-vivos no bestiário |

## Trabalho restante para fechar C0

1. Aprovar IDs persistíveis e esquema de dados dos 30 nós, sem acoplar ao nome localizado, tecla ou Blueprint. Validar árvore, custos e pré-requisitos contra GDD 01 e decisões D2/D5.
2. Para cada classe, fechar turnaround/rig, quatro animações ativas, estados das duas passivas, ícones, VFX, SFX e variantes reduzidas. Definir reutilização de rig/material sem tornar as cinco silhuetas iguais.
3. Definir alcance, alvo, projeção 3D e colisão por habilidade; esses números espaciais não existem na ficha 2D. Medir na câmera antes de aprovar dano/tempo em combate.
4. Montar catálogo de equipamento que garanta o A1 de cada herói e suporte as demais habilidades; [lojas regionais](regional-commerce-3d.md) continuam propostas até preços e itens simulados.
5. Produzir testes de equivalência de regra por classe somente quando gameplay Unreal for iniciado, após gate C0. A ficha não prova que magia, IA, invocação ou passivas já funcionam.
