# Núcleo não visual v2 — inventário, XP e Bastião

**Estado em 16/10/2026:** código C# compilado/testado em Mono, independente de Unity. Incremento sobre o agregado do PR #30. Isto não é um cliente jogável ou validação do combate físico. Fonte principal: `shared/csharp/Aethel.Campaign/`.

## O que foi implementado

- Inventário com instâncias próprias, bolsa em grade, baú por personagem/campanha, pilhas, divisão/fusão, movimentação, dois slots de equipamento do catálogo existente e reserva persistente.
- Entrega real da recompensa do Bosque na bolsa ou reserva, junto da escolha; o antigo contador isolado deixa de ser usado no schema 2.
- XP por derrota confirmada da primeira rota, curva até o nível 30, subida múltipla, pontos de atributo, desbloqueios básicos e pontos de especialização derivados do nível. Level-up/atributos não curam.
- HP/MP persistidos, consumíveis, cooldown compartilhado, antídoto, regeneração de MP por tempo de simulação e recuperação autorizada no hub.
- Guarda/Impacto do Bastião, Postura Firme e oito modificadores dos dois caminhos de especialização, com snapshots por execução. Matemática determinística de dano direto e projeção de ruptura.
- Dano de HP confirmado por adaptador confiável, deduplicado por sequência; morte cancela ações pessoais no mesmo commit. Contexto de hub recusa dano hostil.
- Migração do formato experimental C# schema 1 para 2, com reserva preservada e backup na primeira escrita. Não lê nem altera saves Godot.
- Janela limitada de recibos com recusa de retry expirado, evitando a paralisação da progressão ao chegar a 512 comandos.

## Inventário e propriedade

O contrato de grade/baú G21 prevalece sobre a bolsa de contagens do Godot. **Baseline espacial NOVO/EXPERIMENTAL:** bolsa 10×6, baú 10×10, rotação desativada; machados 1×3, peitorais 2×3, consumíveis/materiais 1×1. É uma escolha de implementação para testes, não dimensão visual aprovada. O limite de peso Godot não é importado implicitamente para a grade; a capacidade física é a ocupação espacial. O baú pertence ao mesmo personagem/campanha, sem compartilhamento.

Os 16 templates existentes são gerados a partir dos `.tres`, preservando IDs, tipo, stacks, poder, armadura e restauração. Não foram criados afixos ou equipamentos das outras classes por inferência. Equipamento usa uma instância por unidade; consumível/material usa quantidade até 20. Slots atuais: `main_hand` e `chest`, conforme o catálogo portado. O template de machado só pode ser equipado pelo Bastião. Os demais slots/famílias do GDD ainda dependem de catálogo e implementação; não declarar suporte a anéis/duas mãos neste incremento.

Item equipado continua na bolsa e é uma referência. Para mover ao baú, desequipar primeiro. Arrastar para fora, sobrepor ou usar instância alheia é recusado; falha mantém posição, quantidade, ouro e referência anteriores. Divisão cria ID determinístico do serial persistido, sem reaproveitar ID de um item consumido. As identidades são estáveis por personagem/campanha/serial.

Concessão preenche pilhas compatíveis da bolsa em ordem estável e busca o primeiro espaço livre. Excedentes entram na reserva, limitada a 100 instâncias. Se nem a reserva comporta a recompensa, o candidato inteiro é recusado, inclusive uma fusão parcial que já tivesse sido tentada. O início do encontro/recipiente deve consultar capacidade no futuro adaptador; este incremento não cria esse loop de mundo. Itens essenciais de quest usam compartimento separado, sem ocupar grade; limites estruturais validados não são peso/células.

Coleta da reserva pode preencher parte de uma pilha e manter o restante reservado, sem perder propriedade. Baú/compras exigem acesso confiável de hub. Só foram portadas compras já precificadas: poção de vida por 10 ouro e frasco de Éter por 12. Venda, comércio completo, todos os slots, afixos, loot aleatório e itens lendários não são declarados prontos.

## XP, atributos e recursos

Curva existente: próximo nível `100 + 50 × (L−1)`; mínimo acumulado para nível L é `100 × (L−1) + 25 × (L−1) × (L−2)`. Nível 5 exige 700 XP e nível 30 exige 23.200. Esses valores são experimentais existentes, agora executáveis; não comprovam ritmo/equilíbrio da campanha. XP além do cap não gera níveis ou pontos extras.

Primeira rota: Lobo 10 XP, Cultista 12, Garras de Musgo 200. `IGameplayAccess` precisa confirmar a derrota e identificar um spawn estável, não apenas o arquétipo. Mesmo spawn não concede novamente com outro request ou após reload. O catálogo dos demais inimigos/zonas precisa ser produzido antes de habilitar suas recompensas; ID desconhecido não recebe XP inventada. O método antigo de elevar nível diretamente tornou-se interno para testes/compatibilidade, sem virar comando de UI.

Três pontos de atributo por nível, base de classe preservada, teto de 99. Fórmulas HP/MP/DEF seguem o GDD. Alocar ou subir nível aumenta máximos quando devido, sem elevar recursos atuais. Desbloqueios básicos respeitam níveis/pré-requisitos e orçamento separado; sua disponibilidade de execução ainda depende de a habilidade estar portada.

Vida: até 50 HP; Éter: até 30 MP; cooldown compartilhado de 5 s. Antídoto só remove veneno removível e tem cooldown próprio de 5 s; não remove Bruma/quest. Recurso cheio, ausência de efeito, item fora da bolsa, morte ou cooldown recusam sem consumo. Débito do item, recuperação e timer são gravados juntos.

MP regenera `1 + floor(INT/10)` por segundo, somente com evidência de ausência de combate/DoT, após 5 s sem hostilidade e 3 s sem gasto. Frações usam milissegundos inteiros; HP não regenera naturalmente. O adaptador deve registrar as mudanças de combate na ordem da simulação. `AdvanceSimulationTo` é monotônico e limita cada salto a 60 s, sem relógio de parede. Não criar gravação de JSON a cada frame de renderização: o controlador deve agrupar a simulação e persistir nos marcos duráveis definidos; desempenho de I/O ainda exige medição na integração.

Recuperação no hub exige autorização específica, cura HP/MP e remove veneno/ações transitórias. Não zera cooldowns para permitir alternância/exploit. O comando não implementa deslocamento/respawn; o adaptador deve confirmar chegada à âncora segura. Dano hostil também exige autorização explícita, mesmo após veneno/morte; nunca aplicá-lo no hub.

## Efeitos de especialização do Bastião

Valores centralizados em `GameplayBalance.cs`; os tiers posteriores são **NOVOS/EXPERIMENTAIS** definidos por delegação do proprietário. Os caminhos das outras quatro classes continuam sem efeitos executáveis; seus nós só modelam alocação e expõem `HasImplementedEffect = false`.

| Caminho / tier | Efeito não visual |
|---|---|
| Defesa t1 | Bloqueio frontal durante Guarda prepara por 3 s +15% aditivo no próximo Impacto validado; cooldown interno de 6 s |
| Defesa t2 | +5 pontos percentuais de redução da Guarda; combinado com Postura Firme, respeita teto global de 60% |
| Defesa t3 | Custo da Guarda passa de 8 para 6 MP |
| Defesa t4 | Bônus do contra-ataque passa de +15% para +30%, sempre dentro do teto aditivo global |
| Ruptura t1 | Impacto com dano confirmado a HP/barreira aplica −15% DEF por 5 s; metade em subchefe/boss |
| Ruptura t2 | Custo do Impacto passa de 12 para 9 MP |
| Ruptura t3 | Ruptura dura 7 s |
| Ruptura t4 | Ruptura passa a −20% DEF, metade em subchefe/boss |

Postura Firme usa o primeiro bloqueio válido da Guarda para +10 pontos percentuais de redução por 2 s. Um bloqueio de costas não prepara carga. Comando recusado não consome carga/MP; execução aceita consome a carga mesmo sem alvo. Retry não cobra ou prepara novamente.

O Impacto captura nível, atributo, arma, bônus de contra-ataque, potência e duração da ruptura antes da apresentação. Trocar equipamento, alocar atributo ou fazer respec depois não reescreve uma ação emitida. Hit confirmado deve referenciar essa execução e horário dentro da janela ativa de 80 ms. O efeito começa no tempo do hit, sem prolongar duração por atraso de entrega. Hit repetido do mesmo ataque/alvo não reaplica/renova. Defesa do primeiro golpe é calculada ANTES de confirmar a ruptura; `ImpactDamage` usa `CombatMath` e a defesa vigente. O adaptador físico não pode recalcular retrospectivamente o dano para HUD depois de aplicar o debuff.

Ruptura não altera MDEF/resistências nem gera stun de boss. Reaplicação não acumula; prevalece a maior redução ativa e renova duração. Cache de ações retém até 32 execuções recentes; fatos antigos de bloqueio expiram e exigem timestamp compatível com a Guarda atual. Debuffs vencidos são retirados ao processar novas ações. Eventos de mundo devem ser entregues na ordem da simulação; uma ação aposentada não autoriza um novo hit.

O núcleo calcula modificadores e recebe evidências confirmadas; não implementa alcance, colisão, fases de animação, RNG de crítico, IA ou vida dos atores inimigos. O controlador físico ainda precisa resolver/aplicar esses resultados na transação de combate apropriada. Receber um `HitEvidence` arbitrário da UI invalidaria a autoridade do sistema. Matemática pura não equivale a todos os ataques das cinco classes implementados.

## Persistência, migração e retries

Schema 2 armazena inventário, XP, atributos, HP/MP, timers, desbloqueios, ações e derrotas junto da campanha. Copy profundo mantém snapshots isolados. Dados novos inválidos/sem catálogo não viram equipamento silenciosamente.

Na leitura do schema 1 do protótipo C#, validar campos antigos antes de migrar. O nível é convertido ao mínimo de XP correspondente, porque o formato antigo não guardava XP. Reservas viram instâncias sem duplicação. Novos recursos são inicializados nos máximos derivados, pois o formato antigo não tinha HP/MP; pontos de atributo ficam não distribuídos. O Bastião recebe somente o kit inicial documentado, com IDs determinísticos. As demais classes não recebem armas novas inventadas. Campos extras não pertencentes ao schema antigo não são tomados como autoridade.

Uma leitura não sobrescreve o arquivo. A primeira escrita válida salva schema 2 com backup do original schema 1; falha preserva o original. Não é conversor de save Godot, nem do JSON isolado de memória, nem promessa de compatibilidade com um schema 2 futuro alterado sem migração.

Recibos dos últimos 512 comandos são mantidos com versão de origem. Compactar um recibo avança um piso persistido. Retry antigo fora da janela retorna `Expired` sem executar; dentro dela, mesmo token/payload retorna `Replayed`, alteração conflita. O cliente deve preservar token, payload E versão de origem no retry. Não trocar a versão para reapresentar uma operação expirada: consultar estado e reconciliar o resultado antes de iniciar outra intenção. Um UUID deve ser novo para cada nova intenção. Recibos compactados não podem ser anunciados como replay ilimitado ou resposta histórica eterna.

Derrotas por spawn permanecem protegidas separadamente (limite estrutural de 8.192 nesta versão). Eventos de dano usam sequência persistida crescente, para impedir reaplicação após recuperação. Estados que ultrapassem capacidade são recusados; política de novas expedições/arquivamento precisa de tratamento antes de liberar conteúdo acima desses limites. Arquivo JSON continua limitado a 1 MiB; falha de capacidade não confirma recompensa parcial.

## Como validar

```bash
python3 tools/generate_campaign_content.py
python3 tools/generate_item_catalog.py
python3 tools/generate_runtime_rewards.py
python3 tools/generate_world_memory_catalog.py
bash tools/run_campaign_tests.sh
bash tools/run_world_memory_tests.sh
python3 -m unittest discover -s tools/tests -q
```

Executado em Mono 6.8.0.105/C# 7.2 com warnings como erros. O harness de runtime cobre grade, limites, propriedade, reserva cheia/parcial, compras, consumo/cooldowns, XP/retries, vetores de dano, snapshots/respec, tiers do Bastião, sessão longa, morte/hub e migração/reload. As matrizes anteriores continuam em regressão. A CI Windows/.NET 8 foi atualizada para compilar os mesmos fontes e executar o harness, sem afirmar que seu resultado remoto ou Unity foram validados aqui.

Ainda exigem trabalho próprio: outros kits/classes, loot/afixos/slots futuros, catálogo de XP da campanha, combate físico/IA, itens de quest adicionais, recuperação guiada e testes de desempenho/plausibilidade. As telas não são a única parte restante do jogo inteiro; este incremento fecha o núcleo não visual do piloto no escopo declarado.
