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
