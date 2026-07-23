# Sprint C – Definition of Done (DoD)

Este **Definition of Done (DoD)** estabelece, de forma exaustiva e objetiva, os critérios que determinam quando **cada User Story do Sprint C** pode ser considerada concluída. O DoD está organizado **por módulo** e **por User Story**, garantindo rastreabilidade, qualidade técnica, conformidade académica e alinhamento com Scrum.

---

## 1. DoD Global (aplicável a TODAS as US do Sprint C)

Uma User Story só é considerada **DONE** quando:

* Todos os Acceptance Criteria estão totalmente cumpridos.
* Código implementado, revisto e integrado na branch principal.
* Não existem erros críticos nem bloqueios conhecidos.
* Testes unitários e de integração relevantes executados com sucesso.
* Logs e auditoria implementados quando exigido.
* Segurança (IAM, RBAC/ABAC) validada.
* Documentação técnica atualizada.
* Funcionalidade demonstrável no SPA (quando aplicável).

---

# 2. Operations & Execution Management (OEM)

### US 4.1.1 – OEM como serviço independente

DONE quando:

* Serviço backend autónomo criado e executável.
* Arquitetura modular respeitada.
* API REST CRUD completa para todos os conceitos OEM.
* Swagger/OpenAPI disponível e validado.
* Comunicação exclusiva via REST.
* Autenticação e autorização ativas.
* Testes de integração entre módulos executados.

### US 4.1.2 – Geração e armazenamento de Operation Plans

DONE quando:

* Geração automática por dia funcional.
* Integração com módulo de Scheduling validada.
* Visualização no SPA antes da persistência.
* Metadados (autor, data, algoritmo) guardados.
* Persistência validada em base de dados.

### US 4.1.3 – Pesquisa e listagem de Operation Plans

DONE quando:

* Endpoint com filtros por período e navio.
* Ordenação funcional.
* Tabela no SPA com dados corretos.

### US 4.1.4 – Atualização manual de Operation Plans

DONE quando:

* Atualizações validadas via API.
* Auditoria (timestamp, utilizador, motivo) registada.
* Alertas de inconsistência apresentados.

### US 4.1.5 – VVNs sem Operation Plan

DONE quando:

* Endpoint identifica corretamente VVNs sem plano.
* Vista dedicada no SPA.
* Regeneração com confirmação explícita.

### US 4.1.6 – Utilização de recursos

DONE quando:

* Agregação correta por recurso e período.
* Cálculo validado de tempos e operações.
* Visualização clara no SPA.

### US 4.1.7 – Criação de Vessel Visit Execution (VVE)

DONE quando:

* VVE criada associada a VVN.
* Estado inicial "In Progress".
* Identificador gerado automaticamente.

### US 4.1.8 – Atualização de berth e dock

DONE quando:

* Atualização persistida corretamente.
* Aviso automático para desvios do plano.
* Log de auditoria registado.

### US 4.1.9 – Registo de operações executadas

DONE quando:

* Operações ligadas ao plano.
* Estados sincronizados.
* Tempos reais registados.

### US 4.1.10 – Pesquisa e listagem de VVEs

DONE quando:

* Filtros funcionais.
* Métricas calculadas corretamente.
* Apresentação consistente no SPA.

### US 4.1.11 – Encerramento de VVE

DONE quando:

* Unberth e saída do porto registados.
* Validação de operações concluídas.
* VVE bloqueada para edição.

### US 4.1.12 – Gestão de Incident Types

DONE quando:

* CRUD completo.
* Hierarquia funcional.
* Severidade e códigos únicos validados.

### US 4.1.13 – Gestão de Incidentes

DONE quando:

* CRUD funcional.
* Associação correta a VVEs.
* Cálculo automático de duração.

### US 4.1.14 – Gestão de Complementary Task Categories

DONE quando:

* CRUD completo.
* Categorias disponíveis para uso.

### US 4.1.15 – Gestão de Complementary Tasks

DONE quando:

* Registo de tarefas paralelas/bloqueantes.
* Associação correta à VVE.
* Estados atualizados corretamente.

---

# 3. 3D Visualization

### US 4.2.1 – Seleção de instalações portuárias

DONE quando:

* Object picking funcional.
* Câmara centra corretamente o objeto.
* Feedback visual aplicado.

### US 4.2.2 – Seleção de navios e recursos

DONE quando:

* Seleção funcional para todos os tipos.
* Câmara e foco corretos.

### US 4.2.3 – Overlay informativo

DONE quando:

* Tecla "i" ativa/desativa overlay.
* Dados atualizados conforme papel.

### US 4.2.4 – Estado operacional visual

DONE quando:

* Estados refletidos visualmente.
* Tooltips explicativos.

### US 4.2.5 – Spotlight dinâmico

DONE quando:

* Luz acompanha seleção.
* Penumbra visível.

### US 4.2.6 – Transições suaves

DONE quando:

* Animações suaves.
* Interface permanece responsiva.

### US 4.2.7 – Reset de câmara

DONE quando:

* Posição default restaurada.
* Animação aplicada.

---

# 4. Scheduling & Planning

### US 4.3.1 – Algoritmo genético

DONE quando:

* Algoritmo implementado e parametrizável.
* Resultados comparáveis aos existentes.
* Disponível no SPA.

### US 4.3.2 – Seleção automática de algoritmo

DONE quando:

* Política implementada.
* Algoritmo selecionado indicado nos resultados.

### US 4.3.3 – Rebalanceamento de docks

DONE quando:

* Rebalanceamento funcional.
* Comparação antes/depois visível.
* Alterações auditadas.

### US 4.3.4 – Estudo Robotics & Computer Vision

DONE quando:

* Relatório entregue (2–3 páginas).
* Estado da arte analisado.
* Integração futura discutida.

---

# 5. Systems Administration & Business Continuity (4.4)

### US 4.4.1 – Disaster Recovery (DR) Plan

DONE quando:

* Plano de Disaster Recovery documentado formalmente.
* Procedimentos de recuperação passo-a-passo descritos.
* Simulação de recuperação executada com sucesso.
* Evidência de que RTO e RPO cumprem o MBCO definido no Sprint B.
* Resultados da simulação registados.

### US 4.4.2 – Infraestrutura para MTD de 20 minutos

DONE quando:

* Infraestrutura proposta descrita (arquitetura, componentes, dependências).
* Justificação técnica clara para cumprir MTD ≤ 20 minutos.
* Avaliação de impacto em custos, desempenho e manutenção incluída.
* Decisão arquitetural documentada.

### US 4.4.3 – Business Impact Analysis (BIA)

DONE quando:

* BIA atualizada com base na solução final.
* Componentes críticos identificados.
* Impactos de falha classificados (operacional, financeiro, legal).
* Riscos do Sprint B reavaliados (mitigados, residuais, novos).
* Documento estruturado entregue.

### US 4.4.4 – Validação de Access Management

DONE quando:

* Testes de acesso executados para todos os papéis.
* Princípio do menor privilégio validado.
* Logs de acessos bem-sucedidos e falhados disponíveis.
* Relatório de revisão de segurança produzido.

### US 4.4.5 – Clustering / Load Balancing do SPA

DONE quando:

* Solução escolhida implementada (proxy, cluster ou orquestração).
* Failover testado com evidência.
* Balanceamento de carga demonstrado.
* Resultados de performance documentados.

### US 4.4.6 – Script de Backup Automatizado

DONE quando:

* Script automatizado funcional.
* Backups criados com formato <db_name>_yyyymmdd.
* Execução automática configurada.
* Logs de sucesso e erro registados.

### US 4.4.7 – Política de Retenção de Backups

DONE quando:

* Script aplica corretamente retenção diária, semanal e mensal.
* Backups antigos eliminados conforme regras.
* Ações registadas em logs.

### US 4.4.8 – Logging e Alertas de Backup

DONE quando:

* Eventos de backup registados nos logs do sistema.
* Alertas configurados para falhas críticas.
* Alerta removido automaticamente após backup bem-sucedido.

### US 4.4.9 – Eliminação Automática de Backups Antigos

DONE quando:

* Backups diários >7 dias removidos.
* Backups mensais/anuais preservados.
* Execução automática validada.

### US 4.4.10 – SSH com Autenticação por Certificado

DONE quando:

* Autenticação por password desativada.
* Apenas chaves autorizadas permitem acesso.
* Testes de acesso documentados.

### US 4.4.11 – Diretório de Rede Partilhado

DONE quando:

* Diretório configurado via SMB/NFS.
* Permissões corretamente definidas.
* Guia de acesso para Linux e Windows disponível.

### US 4.4.12 – Teste Automático de Restauro de Backups

DONE quando:

* Script restaura backup para base temporária.
* Validação automática de integridade executada.
* Resultados registados em log.
* Falhas disparam alertas.

---

# 6. GDPR – Awareness & Data Impact (4.5)

### US 4.5.1 – Gestão da Privacy Policy

DONE quando:

* Política atual publicada no sistema.
* Histórico de versões preservado.
* Notificação apresentada aos utilizadores após alterações.
* Acesso disponível a partir do SPA.

### US 4.5.2 – Informação sobre Tratamento de Dados

DONE quando:

* Secção de Privacy Policy acessível a utilizadores e não utilizadores.
* Conteúdo em conformidade com artigos 13 e 14 do GDPR.
* Informação clara sobre dados indiretos (ex.: tripulação).
* Linguagem clara e transparente.

### US 4.5.3 – Exercício de Direitos dos Utilizadores

DONE quando:

* Secção "Data Rights" disponível no SPA.
* Pedido de acesso/exportação de dados funcional.
* Pedido de retificação e apagamento funcional.
* Registo e confirmação dos pedidos.

### US 4.5.4 – Direitos de Não Utilizadores

DONE quando:

* Procedimento descrito na Privacy Policy.
* Contactos e passos claramente definidos.

---

# 7. Project Client Analysis (4.6)

### US 4.6.1 – Identificação e Caracterização de Segmentos de Mercado

A User Story é considerada **DONE** quando:

* Os segmentos de mercado relevantes para o produto estão claramente identificados.
* Cada segmento é caracterizado considerando:

    * Tipo de organização (ex.: autoridade portuária, operador logístico, terminal privado).
    * Dimensão e relevância do mercado.
    * Necessidades e problemas específicos que o produto resolve.
    * Grau de maturidade tecnológica.
* O(s) segmento(s)-alvo são escolhidos e **justificados de forma explícita**.
* Segmentos não selecionados são identificados e justificados.
* A análise é coerente com as funcionalidades efetivamente desenvolvidas no projeto.
* O texto apresenta estrutura lógica, clareza e linguagem académica.

---

### US 4.6.2 – Definição do Marketing Mix

A User Story é considerada **DONE** quando:

* O Marketing Mix é analisado de forma completa e estruturada:

    * **Produto**: proposta de valor, funcionalidades-chave e diferenciação.
    * **Preço**: modelo de pricing adotado (licença, subscrição, SaaS, etc.) e respetiva justificação.
    * **Distribuição (Place)**: canais de venda, entrega e suporte ao cliente.
    * **Promoção**: estratégias de comunicação, aquisição e retenção de clientes.
* Existe coerência entre todos os elementos do Marketing Mix.
* As decisões estão alinhadas com os segmentos definidos na US 4.6.1.
* A argumentação é lógica, consistente e orientada ao mercado.
* O conteúdo demonstra compreensão de conceitos fundamentais de marketing.

---

### US 4.6.3 – Análise Financeira (Balanço e Demonstração de Resultados)

A User Story é considerada **DONE** quando:

* O **Balanço (BS)** é apresentado de forma correta e completa.
* A **Demonstração de Resultados (IS)** é apresentada de forma correta.
* Existe comparação entre dois períodos (ano *n* e ano *n-1*).
* As principais variações financeiras são identificadas.
* As variações são explicadas com base em decisões de negócio e contexto do projeto.
* As principais suposições financeiras são explicitadas.
* A informação é apresentada de forma clara, com tabelas legíveis e comentários analíticos.

---

### US 4.6.4 – Análise de Rácios Financeiros e Avaliação de Desempenho

A User Story é considerada **DONE** quando:

* Os principais rácios financeiros são corretamente calculados, incluindo:

    * Liquidez.
    * Solvabilidade.
    * Rentabilidade.
    * Eficiência.
* As fórmulas aplicadas estão corretas.
* Os resultados são **interpretados**, não apenas apresentados.
* Existe comparação temporal ou com valores de referência.
* As conclusões são relacionadas com a viabilidade económica do projeto.
* A ligação entre rácios, BS e IS é clara e consistente.

---

### US 4.6.5 – Avaliação Global do Cliente e Sustentabilidade do Projeto

A User Story é considerada **DONE** quando:

* O perfil do cliente ideal está claramente definido.
* Os principais benefícios do produto para o cliente são identificados.
* Os riscos de mercado e barreiras à adoção são analisados.
* A sustentabilidade económica do projeto é discutida.
* A coerência estratégica do produto é avaliada.
* Existe uma conclusão global que sintetiza toda a análise do módulo.
* A argumentação final é lógica, bem estruturada e fundamentada.

---


## 8. Done Final do Sprint C

O Sprint C está concluído quando:

* Todas as User Stories cumprem integralmente o respetivo DoD.
* Evidências técnicas e documentais disponíveis.
* Sistema integrado, demonstrável e auditável.

O **Sprint C** está concluído quando:

* Todas as US comprometidas cumprem o respetivo DoD.
* Demonstração integrada possível.
* Qualidade técnica e documental validada.
