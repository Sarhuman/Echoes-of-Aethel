# Decisões de produção — vertical slice offline

> **Atualização de direção, 14/10/2026:** o usuário escolheu preparar um novo cliente 3D em Unreal. A decisão antiga de Godot/GDScript definitivo abaixo descreve a primeira fatia já jogável, não a engine do próximo cliente. O [plano de transição](unreal-transition.md) rege somente engine/pipeline; regras de domínio deste documento continuam válidas.

**Fonte:** definições do Game Director fornecidas em 13/10/2026. **Estado:** regras aprovadas para a primeira fatia, com implementação Godot parcial/completa conforme cada módulo; próximo cliente 3D em preparação. Aprovação de design não significa que todo o GDD já seja executável.

## 1. Entrega e plataforma

- Cliente Godot/GDScript validado para a primeira fatia; novo cliente 3D planejado em Unreal conforme `unreal-transition.md`. Windows desktop primeiro; Web e Android em marcos posteriores.
- Slice: Bastião, Cidadela, Bosque, Lobo Etéreo, Cultista Cego-Guiado (nome curto Cultista Cego), Garras de Musgo, A Caça ao Espião e degradação inicial do hub.
- Core da primeira fatia: combate básico, inventário com capacidade/peso, IA, comércio limitado na Cidadela e save local. Outras classes, mapas 2–12, multiplayer e crafting avançado ficam fora do slice.
- Top-down 3/4 com perfil de projeção 2:1. Input lógico limitado/normalizado; projeção `(x, y×0,5)`. Distância vertical na tela é menor por definição. Colisores/mapa/percepção precisam usar a métrica escolhida coerentemente; aplicar 0,5 só na velocidade não transforma automaticamente toda a física em isométrica.

## 2. Recursos e dash

HP e MP são universais; todas as ativas do Bastião voltam a usar MP (Guarda 8, Impacto 12, Fenda 20). Ataque básico é gratuito. A quarta barra de vigor/fúria é removida do desenho; fúria será buff passivo após dano, com potência/duração/teto ainda a definir antes de implementar.

Stamina universal paga somente dash: máximo 100 e custo 25. Manter duração 0,2 s/cooldown 1,2 s do baseline até revisão explícita. Invulnerabilidade de dano nos primeiros **120 ms**, controlada por tempo de simulação, nunca por contagem de frames de renderização. Pode cancelar recuperação do ataque normal; não windup pesado.

I-frames ignoram dano/hurtboxes hostis; **paredes, chão e limites de mapa continuam sólidos**. Desligar todas as colisões permitiria atravessar cenário e não será tratado como requisito. Ordem entre input/dano no mesmo passo deve ser determinística e testada. Dash não reseta cooldown ou multiplica dano do ataque cancelado.

## 3. Bruma e recuperação

Modificador adicional de corrupção tem teto de +25% HP e dano por ato, aplicado uma única vez à base correspondente. Não empilhar sobre promoção anterior ou multiplicar +25% novamente a cada respawn. Isso substitui a promoção +50% HP do módulo 05.

Hub e faixa inicial de saída nunca geram elites. Bordas de recuperação usam comuns com taxa aumentada de poções após mortes consecutivas na mesma zona. Número de mortes, bônus de chance, teto e critério de reset exigem catálogo/teste antes da implementação; nunca aumentar por repetir reload. Loot econômico base e recompensas únicas permanecem idempotentes.

## 4. Offline e Alpha

Slice e lançamento single-player não exigem combate autoritativo em tempo real no backend. Alpha adiciona FastAPI para autenticação, saves em nuvem e telemetria de escolhas. Sincronizar saves é diferente de validar verdade de combate: hash/criptografia locais não comprovam uma vitória.

Se forem adicionadas instâncias online com recompensas competitivas, definir protocolo de validação antes de anunciar anticheat de drops. Sortear loot no servidor a partir de uma vitória declarada pelo cliente não resolve fraude. Não introduzir ranking/economia compartilhada com confiança em snapshots offline.

Save atual (PR #12) usa JSON + SHA-256 e backup, **não criptografia**. Criptografia local solicitada permanece trabalho pendente: escolher origem da chave, recuperação e compatibilidade Windows antes de usar open_encrypted_with_pass. Chave fixa no executável/XOR não blinda adulteração. Preservar migração e recuperação dos saves existentes ao trocar formato.

## 5. Crime e redenção

Cidadãos externos vulneráveis podem receber dano. NPCs essenciais não são removidos permanentemente; tentativa de agressão pode recusar interação e chamar guarda, sem autorizar combate/dano no hub. Hostilidade por quest/Bruma permite legítima defesa sem crime ou perda de reputação.

Testemunhas usam visão/som com raio de referência 250 pixels. CrimeService confirma agressor/vítima/zona/posição e ID antes do EventBus. Visão exige linha de visão; audição exige política de alcance/oclusão explícita. Raio projetado em pixels e alcance lógico não devem ser confundidos.

Indulgência ou quest de facção pode converter hostile → alive/neutro, mas nunca dead → alive. CrimeService registra causa da hostilidade e resolução para não apagar agressão/maldição incompatíveis. O save atual guarda status, mas ainda precisa desse contexto. FSM atual não implementa essa redenção ou reputação completa.

## 6. Escolhas, personagens e finais

Ambas as rotas oferecem ganhos reais: misericórdia favorece civis e bênçãos; pragmatismo oferece ouro/item raro/bônus crítico. Valores, duração e unicidade entram no catálogo; nenhuma conversa/reload repete ganho. Investigar permite prever o fardo, sem oferecer uma terceira solução limpa. Evitar pistas que contradizem o resultado confirmado.

Bram: ferreiro rúnico/paranoia. Lyra: pesquisa e dependência de Éter. Irian: líder político do conselho, preservando representação dos refugiados/arquivo quando compatível. Vael é entidade paterna dos heróis.

`restore_order` passa a ter título **A Restauração da Ordem / O Fim da Ruína**. `embrace_ascension` permanece **A Ascensão Estelar**, podendo usar **O Fim da Ordem** como subtítulo. Títulos não trocam IDs/efeitos já definidos; alinhar o roteiro antes de produção de cenas. “Com Vael” descreve continuidade do projeto/legado: não implica ressuscitar seu corpo após a vitória sem nova decisão narrativa.

## 7. Governança e realidade dos PRs

As etapas propostas “PR #1–#5” são **sequência de trabalho**, não os números reais do GitHub. Os PRs reais #1–#11 construíram documentação; #12 implementa save/core mínimo; #13 implementa ActorBase/FSM. Não afirmar que #1 contém SaveSystem ou que Player/Inventário já existem.

| Etapa de produção | Situação |
|---|---|
| Core e persistência | PR #12; ActorBase chega com #13; ampliar schema e política de criptografia |
| Player, projeção e dash | Próxima implementação; aplicar recursos e i-frames aprovados |
| IA/NPC | PR #13 é núcleo offline; NPC Core/CrimeService e integração com Player pendentes |
| Inventário/peso | Ainda não implementado; definir pesos/capacidade sem bloquear itens de quest |
| Quest/diálogo do Ato I | GDD existente; resolver/cenas/runtime pendentes |

Integrar PRs reais por dependência/base, revisando diffs e checks, não pela numeração abstrata das etapas. Não reescrever histórico nem renumerar PRs. Game Director aprova arquitetura/código e narrativa/balanceamento; EventBus notifica fatos, sem substituir serviços explícitos ou validação.

## 8. Próximo marco verificável

Player offline e cena mínima Windows: movimento 2:1, parede sólida, ataque normal, recovery cancelável, windup pesado não cancelável, dash de 120 ms sem dano e barras HP/MP/stamina. Integrar FSM e depois inventário/quest. Manter testes atuais; adicionar testes de recursos/dash, não alegar que os 52 checks antigos validam estas decisões novas.


## 9. Especialização de classe — decisão de 16/10/2026

O proprietário solicitou incorporar a proposta de especializações: primeira escolha a partir do nível 5, aviso discreto sem interromper combate, comparação acessível no menu e confirmação via NPC em hub seguro. Bram conduz o Bastião. Adiar a seleção não bloqueia progressão. O nível 5 é referência inicial, sujeita à validação da curva de XP; não promete um mapa ou minuto exato.

A [ficha de especializações](../design/class-specializations.md) complementa o GDD 01 e rege esta extensão. A primeira seleção não consome os pontos da árvore básica; A1–A4/P1–P2 e seus desbloqueios permanecem. Especialização não depende de afinidade moral. Respec no hub faz parte da direção; custo/economia, IDs, modificações iniciais e investimentos posteriores precisam de catálogo e testes antes de ativação. Os mentores das outras classes ainda não estão atribuídos. Esta decisão adiciona design ao projeto, não declara cliente Unity ou árvore funcional.


## 10. Formação regional e dificuldade — direção delegada em 16/10/2026

O proprietário delegou resolver a progressão das especializações e a leitura dos níveis. A [ficha de formação/ameaça regional](../design/regional-threat-and-training.md) rege faixas experimentais de monstros por mapa, comparação por número + cor, hubs seguros e responsáveis locais distintos. Não há autoscaling pelo nível do herói. Chefes/elites mantêm identificação própria, independentemente da comparação de nível. Faixas de inimigo não são as faixas de nível de item do GDD 04.

O [complemento de especializações](../design/class-specializations.md) substitui as pendências de design do item 9 quanto a cronograma e baseline de respec: seleção inicial no 5; mais um ponto no 10/15/20/25/30; quatro nós em cadeia por direção, máximo seis investimentos no total. Primeira redistribuição gratuita, depois custo experimental de 5 vezes o nível em ouro, somente em hub e sem cura/reset de cooldown. Define os dois primeiros modificadores experimentais do Bastião; não altera o combate ativo sem implementação/testes.

Responsáveis de serviço na Cidadela: Bram/Bastião, Irian/Tecelão, Sera/Sentinela, Lyra/Invocadora e Maera/Inquisidor, oferecendo formação compatível com sua função e acesso a registros/exercícios. Outras regiões usam pessoas locais distintas com o mesmo serviço e progresso do personagem; nomes/IDs/roteiros desses novos atores ainda precisam de produção. Não duplicar NPC central nem depender do popup para liberar seleção. Esses números e papéis são decisões de direção para produção; balanceamento, testes e runtime continuam pendentes.


## 11. Núcleo não visual v2 — implementação delegada em 16/10/2026

O proprietário solicitou resolver o restante do núcleo independentemente da integração visual. O [runtime v2](runtime-core-v2.md) rege o incremento executado de inventário/XP/recursos, efeitos dos caminhos do Bastião, migração C# e retries. Grade e seis tiers posteriores do Bastião usam baselines NOVOS/EXPERIMENTAIS registrados em `experimental-values.md`; não alteram o cliente Godot.

O formato C# passa de schema 1 a 2, sem conversor Godot. Recibos usam janela e versão de origem; retries compactados expiram sem executar, não bloqueiam sessões longas. Vida/MP e recompensa são persistidos com o agregado. Não anunciar as outras quatro classes, slots/afixos/loot futuros, IA ou combate físico como implementados por esta entrega. As permissões de hub, derrota e evidências precisam vir do adaptador de simulação confiável, não da UI. Gates de editor/câmera/playtest permanecem abertos.
