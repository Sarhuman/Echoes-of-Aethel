# Arquitetura proposta

Documento inicial complementado pela [Sprint 2](../GDD/02_arquitetura_tecnica.md), referência canônica para stack, API, paperdoll e deployment. Godot/GDScript (Sprint 6) e FastAPI/PostgreSQL estão definidos; nenhuma engine ou infraestrutura está implementada.

## Limites

- **Domínio:** combate, atributos, inventário, progressão, decisões e regras de recompensa. Independente de cenas, áudio e UI.
- **Aplicação:** comandos de ataque, interação, escolha e transição; coordena validação, eventos e persistência.
- **Conteúdo:** catálogos declarativos de classes, mapas, quests, efeitos, inimigos e tabelas de loot, com IDs estáveis e validação de referências.
- **Apresentação:** cenas 2D, animações, efeitos, UI, input e acessibilidade. Consome estado/eventos sem conceder recompensas diretamente.
- **Infraestrutura:** arquivos de save, carregamento de assets e adapters da engine.

A stack revisada na [Sprint 6](../GDD/06_arquitetura_software.md) exige experimento mínimo de input, colisão, iluminação, exportação e save no equipamento-alvo antes de fixar versões. O modelo permite ampliar conteúdo sem exigir um serviço distribuído.

## Persistência e consistência

Save versionado: schema_version, content_version, personagem, inventário, mapa/checkpoint, quests, escolhas, efeitos aplicados, eventos pendentes e recompensas de campanha já concedidas. IDs de instâncias de itens e encontros distinguem loot repetível de recompensas únicas.

Aplicar decisão, efeitos imediatos, eventos futuros e recompensa na mesma atualização de estado, persistida de forma atômica. Repetir um comando já concluído retorna o resultado existente. Um evento atrasado recebe ID derivado de quest/decisão/alvo e não pode disparar novamente após reload.

Gravar arquivo temporário, validar e substituir o save anterior; conservar backup recuperável. Validar tamanho, tipos, intervalos, enumerações e referências ao carregar. Não executar scripts, caminhos ou objetos serializados arbitrários vindos do save. Migrações explícitas e testadas; versões futuras desconhecidas são recusadas sem sobrescrever o arquivo.

Um save local pode ser adulterado pelo dono; não fornece autoridade suficiente para competição online. Caso multiplayer seja aprovado posteriormente, combate, inventário e loot competitivos exigirão autoridade do servidor e novo desenho de sincronização.

## Invariantes verificáveis

1. Hub nunca causa dano nem contém inimigos ativos.
2. Uma decisão narrativa válida tem duas opções; somente uma é confirmada por campanha.
3. Repetir decisão/evento não duplica efeitos, loot ou XP.
4. Derrotar novamente um boss não concede outro fragmento de campanha.
5. Três fragmentos de guardiões liberam a rota final; o fragmento de Vael exige sua derrota.
6. Salvar/carregar preserva escolhas, inventário e consequências pendentes.
7. Diálogos usam conhecimento desbloqueado; spoilers futuros ficam fora da UI.
8. Serviços essenciais sobrevivem à deterioração narrativa do hub.
9. Modificadores têm limites e expiração explícitos; nenhuma escolha bloqueia o fim da campanha.
10. Respawn não desfaz quests ou restaura NPCs mortos na história.

## Desempenho e expansão

Simulação com passo fixo separado da renderização; limites de entidades, projéteis, invocações e luzes por cena. Carregamento por zona, descarte de assets sem uso e reutilização de efeitos quando medições justificarem. Medir tempo de frame e memória no hardware-alvo antes de prometer metas.

Input traduzido em ações remapeáveis, sem regras de combate nos handlers de teclado/toque. RNG injetável para testes de loot e encontros. Conteúdo não contém código executável. Dependências e assets precisam de origem/licença registradas, sem segredos no repositório.
