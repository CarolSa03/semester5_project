# Definition of Done (DoD) — Sprint B (Sprint 2)
**Projeto:** LEI-SEM5-PI-2025-26  
**Sprint:** B  
**Objetivo:** Garantir que todas as User Stories do Sprint B são concluídas com qualidade, testadas, documentadas e integradas no sistema.

---

## 1. Critérios Gerais de Definition of Done

### 1.1. A funcionalidade está totalmente implementada
- Todo o código necessário está escrito, compilado e sem erros.
- A lógica implementada cumpre **todos** os Acceptance Criteria da User Story.

### 1.2. A funcionalidade está testada
- Testes unitários criados e com cobertura mínima adequada (recomendado: ≥ 80% nas áreas críticas).
- Testes de integração executados e validados.
- Testes UI/End-to-End realizados quando aplicável.
- Todos os testes passam sem falhas.

### 1.3. Cumprimento dos requisitos de qualidade
- O código segue os standards definidos pela equipa (naming, arquitetura, SOLID, clean code).
- Não existem *warnings* críticos.
- Não existem *code smells* graves (Lint/Cosmetic issues tolerados se irrelevantes).

### 1.4. Revisão por pares (Code Review)
- O código foi revisto por pelo menos 1 membro da equipa.
- Comentários relevantes foram resolvidos antes de merge.
- A PR está aprovada no GitHub.

### 1.5. Documentação atualizada
- Documentação técnica atualizada (**API docs, endpoints, diagramas, algoritmos**, etc.).
- Manual de UI atualizado (para funcionalidades de front-end).
- Comentários essenciais adicionados no código.

### 1.6. A funcionalidade está integrada no sistema
- Merge realizado na branch principal (develop/main) sem conflitos.
- Build CI/CD passou com sucesso.
- A funcionalidade está operacional no ambiente do sprint (dev/staging).

### 1.7. A funcionalidade está demonstrável
- A equipa consegue demonstrar o funcionamento em Sprint Review.
- Qualquer utilizador autorizado consegue reproduzir a funcionalidade sem instruções especiais.

---

## 2. Critérios Específicos por Área do Sprint B

### 2.1. SPA (User Stories 3.1.x)
- Componentes criados com lógica e validações funcionais.
- UI responsiva de acordo com o layout definido.
- Feedback ao utilizador implementado (sucesso, erro, loading).
- As chamadas às APIs funcionam e os dados são apresentados corretamente.
- As permissões (menus, rotas) respeitam o papel do utilizador.

### 2.2. Autenticação & Autorização (3.2.x)
- Fluxo OAuth2/OIDC funcional (login, logout, token refresh).
- Frontend e backend validam tokens corretamente.
- Acesso negado e mensagens apropriadas implementadas.
- Logs de tentativas inválidas implementados no backend.

### 2.3. Visualização 3D (3.3.x)
- Cena 3D renderiza corretamente no SPA.
- Objetos (docks, vessels, storage areas, cranes) aparecem nas posições corretas.
- Texturas, materiais e luzes aplicados conforme requisitos.
- Controles de câmara (orbit, zoom) funcionam sem jitter.
- Dados carregados dinamicamente via API ou mock consistente.

### 2.4. Scheduling & Planning (3.4.x)
- Algoritmo implementado respeitando todas as regras de planeamento.
- Endpoint funcional no módulo de scheduling.
- Resultados retornam JSON válido e consistente.
- UI mostra o planeamento (tabela e/ou timeline).
- Comparações de heurísticas implementadas (se aplicável).
- Análise de complexidade entregue.

### 2.5. Administração de Sistemas (3.5.x)
- Pipeline CI/CD executa com sucesso e deploy é possível.
- Logs armazenados conforme definido.
- Acesso à solução restrito via rede DEI (VPN/IP whitelist).
- Ficheiro de configuração de endpoints funciona sem reinício.
- Estratégias de backup documentadas e testadas.

### 2.6. GDPR (3.6.x)
- Documentação de dados pessoais completa.
- Fluxos de tratamento descritos.
- Processo de resposta a data breach definido e documentado.

### 2.7. Client Analysis (3.7.x)
- Conclusões fundamentadas e coerentes.
- Fontes devidamente citadas.
- Documentos entregues conforme especificado.

---

## 3. Definition of Done — Checklist Sprint B

| Critério | Estado |
|---------|--------|
| Todos os Acceptance Criteria cumpridos | ☐      |
| Testes unitários criados e a passar | x      |
| Testes de integração completados | ☐      |
| Revisão de código aprovada | x      |
| Documentação atualizada | x      |
| Build CI/CD passou sem erros | x      |
| Funcionalidade integrada na branch principal | x      |
| Demonstração possível e validada | x      |
| Sem bugs críticos conhecidos | ☐      |
| Cumprimento de segurança/autorização | x      |
| Aprovada pelo Product Owner | x      |

---

## 4. Resumo para Relatório

Uma User Story está **Done** quando:
- Implementa 100% dos Acceptance Criteria;
- Está testada (unit, integração, UI);
- O código foi revisto e cumpre padrões de qualidade;
- Está documentada e integrada no sistema;
- Pode ser demonstrada sem falhas;
- É aprovada pelo Product Owner.

---
