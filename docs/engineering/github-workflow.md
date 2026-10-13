# Fluxo de entregas no GitHub

Preferência do projeto: publicar cada entrega concluída com commit e pull request, mantendo documentação e status no GitHub atualizados.

Antes de alterar, ler a branch-base e instruções vigentes. Cada PR descreve comportamento/conteúdo entregue, escopo pendente e validação real. Não declarar runtime, testes ou balanceamento aprovados em entregas somente documentais.

PRs dependentes podem usar a branch do PR anterior como base enquanto ele estiver aberto. Informar a dependência na descrição. Quando o anterior for integrado, atualizar a base e conferir o diff antes de integrar o seguinte.

Publicar uma branch e abrir um PR não implica merge automático. Atualizar a main somente conforme autorização de integração e checks aplicáveis. Não sobrescrever alterações concorrentes; usar atualização de ref com SHA esperado.

Arte, código e conteúdo do repositório são versionados aqui. Saves de jogadores, credenciais e arquivos temporários ficam fora do Git.
