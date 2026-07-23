# Definition of Ready (DoR) — Sprint B (Sprint 2)
**Projeto:** LEI-SEM5-PI-2025-26  
**Sprint:** B  
**Objetivo:** Garantir que as User Stories do Sprint B estão claras, estimáveis e totalmente prontas para desenvolvimento.

---

## 1. Critérios Gerais de Definition of Ready

### 1.1. A User Story está completamente compreendida pela equipa
- A descrição e os Acceptance Criteria estão claros conforme o documento oficial.
- Não existem dúvidas significativas pendentes.

### 1.2. A User Story tem Acceptance Criteria testáveis
- Todos os ACs permitem criar testes unitários, de integração e/ou UI.
- A equipa de testes confirma que consegue validar os critérios.

### 1.3. As dependências estão resolvidas
- APIs, dados, módulos ou componentes necessários já existem ou têm mocks disponíveis.
- Dependências externas ou internas estão identificadas e resolvidas.

### 1.4. Backend e Frontend têm requisitos claros
**Frontend (SPA):**
- Wireframes ou mockups definidos.
- Endpoints necessários identificados.
- Regras de navegação e layout definidas.

**Backend:**
- Estruturas de dados (JSON) definidas.
- Payloads de entrada e saída especificados.

### 1.5. A User Story está estimada
- A US tem story points atribuídos.
- Tamanho adequado para completar na sprint.

### 1.6. Critérios de segurança e autenticação definidos (quando aplicável)
- Tokens, roles e permissões conhecidos.
- Rotas protegidas identificadas.

### 1.7. Testes planeados
- Testes unitários especificados.
- Testes de integração identificados.
- Testes de UI quando aplicável.

---

## 2. Critérios Específicos por Área do Sprint B

### 2.1. SPA (User Stories 3.1.x)
- Framework escolhida (React/Angular/Vue).
- Estrutura modular definida (components, services, routing).
- Layout base criado (header, menu, área de conteúdo).
- Suporte multilingue preparado (PT/EN).
- Sistema de design definido.

### 2.2. Autenticação & Autorização (3.2.x)
- IAM configurado (OAuth2/OpenID Connect).
- Fluxo de login/logout validado externamente.
- Backend pronto para validar tokens.
- Mapeamento de roles concluído.

### 2.3. Visualização 3D (3.3.x)
- Three.js/WebGL integrado no SPA.
- Cena inicial configurada.
- Dados do layout do porto (mock ou real) disponíveis.
- Convenção de modelos e texturas definida.

### 2.4. Scheduling & Planning (3.4.x)
- Endpoints do módulo definidos.
- Acesso aos dados necessários garantido (vessels, docks, cranes, staff).
- Estratégia do algoritmo escolhida (ótimo/heurístico).
- Regras operacionais compreendidas.

### 2.5. Administração de Sistemas & BC (3.5.x)
- Ambiente DEI (VM/container) configurado.
- Pipeline de CI/CD escolhido.
- Regras de rede (VPN, whitelist) estabelecidas.
- Configurações editáveis documentadas.

### 2.6. GDPR (3.6.x)
- Identificação dos dados pessoais recolhidos.
- Fluxo básico de tratamento definido.

### 2.7. Client Analysis (3.7.x)
- Empresa selecionada e validada.
- Fontes de informação recolhidas.
- Documentação base reunida (organigrama, KPIs, relatórios).

---

## 3. Resumo para Relatório

Uma User Story está **Ready** quando:
- É compreendida por todos;
- Tem Acceptance Criteria claros e testáveis;
- Tem estimativa atribuída;
- Todas as dependências estão resolvidas;
- Os requisitos de API e UI foram definidos;
- Critérios de autenticação e autorização identificados;
- Testes planeados;
- É considerada pronta pelo Product Owner.

---
