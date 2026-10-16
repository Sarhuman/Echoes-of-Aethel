# Memória do mundo — primeiro núcleo C#

Implementação isolada e executável em `shared/csharp/Aethel.WorldMemory/`. É a primeira parte da direção “um mundo que se lembra”: escolhas confirmadas produzem fatos imediatos e reações da Cidadela observáveis somente após seus marcos. Não é um cliente Unity, nem uma implementação completa de quests, recursos, recompensas ou das novas interações propostas no PR #25.

## Reuso e autoridade

- `client/godot/quest/quest_service.gd`: preservados investigação antes da escolha do Bosque, escolha irrevogável, replay da mesma opção e confirmação antes de apresentação.
- GDD 04 e manifesto da campanha: doze quests e 24 opções, ordem de desbloqueio, vitória antes de escolhas de bosses de ato/final, afinidades e gatilhos originais.
- Nenhum nome, efeito ou ID narrativo novo. `CanonicalCampaign.cs` é gerado com texto exato de produção. É conteúdo interno; a UI deve consumir somente `MemoryView.Facts`.
- A solicitação de implementação de 16/10 autoriza este incremento de domínio independente. C0 visual/editorial e a criação de cenas funcionais continuam pendentes.

## Comportamento entregue

`MemoryService` aceita pistas do Bosque, entrada de zona, vitória confirmada, escolha, visita à Cidadela e revelação do epílogo. Os adaptadores confiáveis da aplicação devem confirmar pistas, posição e vitória; UI/animação não são autoridades. Não há simulação de combate neste módulo.

Escolhas exigem conclusão das rotas anteriores. Mapas 3/6/9/12 exigem a vitória do próprio encontro antes de decidir. O Bosque exige suas três pistas existentes. As outras evidências de investigação ainda precisam de catálogo aprovado e integração; não são consideradas verificadas por este núcleo.

A mesma escolha retorna `Replayed`; alternativa conflitante é recusada. Versão esperada protege contra comandos concorrentes. Retornos repetidos não duplicam fatos. Afinidades empatadas produzem `mixed`; fatos individuais continuam presentes. Estágio visual deriva de conclusão dos mapas 3/6/9/11, sem relógio real. O epílogo pode ser revelado após Vael e a decisão final sem exigir retorno físico ao hub.

As consequências tardias permanecem fora da projeção pública até visita ao hub após o marco, ou epílogo. Entrada na Cripta/Coração exige comando de entrada real; desbloquear a rota não equivale a entrar. Concluir um ato sem visitar o hub preserva a consequência para a próxima visita. `first_return_to_citadel` é avaliado por quest pendente, inclusive para a escolha dos Esgotos.

Não remove serviços, concede itens, incrementa carga ritual ou aplica modificadores temporários: essas responsabilidades pertencem aos serviços próprios. Os fatos retornados fornecem texto e IDs canônicos para futura composição de cenário, retratos, diálogos e áudio; os objetos visuais e seus mapeamentos ainda não existem.

## Persistência e limites

`JsonMemoryStore` usa JSON schema 1 exclusivo para este núcleo, com identidade de campanha, versão, escolhas, pistas, encontros, entradas e reações já reveladas. Coleções e arquivo são limitados e validados; nenhum script/asset é desserializado. Não aceita save Godot.

Escrita usa arquivo temporário no mesmo diretório, flush, substituição do arquivo com backup e exclusão mútua durante leitura/comparação/gravação. Um candidato de versão obsoleta é recusado; a apresentação lê novamente somente estado confirmado. Arquivo corrompido ou campanha incompatível gera erro, sem iniciar campanha vazia ou recuperar backup silenciosamente. A UI futura precisa oferecer recuperação explícita. O teste de exclusão mútua é local ao processo; concorrência entre processos e durabilidade após desligamento abrupto precisam de validação Windows.

**Integração de produção:** antes de ligar este núcleo à quest jogável, integrar suas alterações à MESMA transação do agregado de campanha que inclui recompensa, ritual e inventário. O arquivo isolado é uma prova de persistência, não autorização para manter duas autoridades de escolha ou confirmar loot em gravação separada. O save global deve ter migração e testes próprios. Não constitui anticheat offline ou validação de vitórias.

## Reproduzir

Ferramentas utilizadas: Mono C# compiler/runtime 6.8.0.105, linguagem C# 7.2 e biblioteca de serialização do framework. Não há pacote de terceiros no código.

Em Linux/WSL com `mono-mcs` e `libmono-system-runtime-serialization4.0-cil` instalados:

```bash
python3 tools/generate_world_memory_catalog.py
bash tools/run_world_memory_tests.sh
```

Para atualizar catálogo após edição aprovada do GDD: `python3 tools/generate_world_memory_catalog.py --write`, revisar diff e repetir testes. A CI dedicada verifica o catálogo e compila/executa o mesmo harness; seu resultado remoto deve ser consultado no PR.

O harness cobre todas as 4.096 combinações no núcleo C#, reload sem retorno entre mapas, escolhas bloqueadas, replay/conflito, empate, falha de armazenamento, isolamento de snapshots, JSON em disco, backup, versão concorrente, arquivo inválido/excessivo, campanha incompatível e bloqueio de arquivo. Isso não testa mapas, câmera, IA, modelos ou frames.

## Próxima integração Unity

1. Executar U0 e fixar versão do editor/pacotes após abrir e empacotar a cena de inspeção no PC real.
2. Configurar assemblies de domínio e infraestrutura com fontes deste módulo, mantendo testes fora de Assets de produção. Verificar `DataContractJsonSerializer` no perfil escolhido, stripping/AOT se aplicável, `File.Replace` e caminhos em Windows; não alegar compatibilidade antes disso.
3. Substituir o armazenamento isolado pelo agregado transacional, portar casos de recompensa/ritual/modificadores e testar falhas entre etapas.
4. Adaptar fatos a estados visuais aprovados: memorial/poço no Bosque, enfermaria no Pântano, raízes no Portal, caravana nas Dunas e assim por diante. Nenhum gatilho de apresentação confirma escolha ou dano.
5. Medir retorno após escolha com jogador: ele reconhece o que mudou e consegue associar a mudança ao que fez? Esse é o aceite de identidade; quantidade de verificações automatizadas não comprova esse resultado.

As dungeons e itens que mudam habilidades continuam no pacote de design e dependem de implementação própria. Não foram adicionadas novas mecânicas ou cenas sob o rótulo de “polimento”.
