# Sistema de Gestão Portuária - SGP GDPR Compliance

**GDPR Awareness & Data Impact Understanding**

**Autores:**
- Duarte Machado, 1240586
- António Freitas, 1240585
- Carolina Sá, 1220623
- Tiago Alves, 1220780
- Luís Cruz, 1241705

**Vídeo:**
Clique na imagem abaixo para ver o vídeo:

[![Assista ao vídeo](https://i9.ytimg.com/vi_webp/yUFuKiVhBvA/mqdefault.webp?v=69231bf1&sqp=CJS3jMkG&rs=AOn4CLBFhIkpYkQO0jDVxDrEFLUWdaEZbQ)](https://youtu.be/yUFuKiVhBvA)


**Instituto Superior de Engenharia do Porto**  
**Instituto Politécnico do Porto**  
**Portugal**  
**Novembro 2025**

---

## Conteúdo

1. [Introdução](#1-introdução)
    - 1.1 [Perfis do Sistema](#11-perfis-do-sistema)
    - 1.2 [Funcionalidades do Sistema](#12-funcionalidades-do-sistema)
2. [Dados Pessoais Processados](#2-dados-pessoais-processados)
    - 2.1 [Tabela Resumida de Dados Pessoais](#21-tabela-resumida-de-dados-pessoais)
3. [Processamento de Dados Pessoais](#3-processamento-de-dados-pessoais)
    - 3.1 [Dados de Staff Members](#31-dados-de-staff-members)
    - 3.2 [Dados de Shipping Agent Representatives](#32-dados-de-shipping-agent-representatives)
    - 3.3 [Dados de Crew Members](#33-dados-de-crew-members)
4. [Fundamentos de Licitude (Artigo 6º do RGPD)](#4-fundamentos-de-licitude-artigo-6º-do-rgpd)
    - 4.1 [Bases Legais por Categoria de Dados](#41-bases-legais-por-categoria-de-dados)
5. [Princípios Gerais do RGPD (Artigo 5º)](#5-princípios-gerais-do-rgpd-artigo-5º)
6. [Medidas de Segurança (Artigo 32º do RGPD)](#6-medidas-de-segurança-artigo-32º-do-rgpd)
7. [Procedimentos em Caso de Violação de Dados Pessoais](#7-procedimentos-em-caso-de-violação-de-dados-pessoais)
    - 7.1 [O que constitui uma Violação de Dados Pessoais](#71-o-que-constitui-uma-violação-de-dados-pessoais)
    - 7.2 [Procedimento de Notificação](#72-procedimento-de-notificação)
    - 7.3 [Prazos Obrigatórios](#73-prazos-obrigatórios)
    - 7.4 [Fluxo de Ação em Caso de Incidente](#74-fluxo-de-ação-em-caso-de-incidente)
8. [Direitos dos Titulares de Dados](#8-direitos-dos-titulares-de-dados)
9. [Avaliação de Impacto (DPIA)](#9-avaliação-de-impacto-dpia)
10. [Conformidade e Responsabilidades](#10-conformidade-e-responsabilidades)
    - 10.1 [Responsabilidades da Organização](#101-responsabilidades-da-organização)
    - 10.2 [Responsabilidades do Project Manager](#102-responsabilidades-do-project-manager)
11. [Recomendações Práticas](#11-recomendações-práticas)
    - 11.1 [Curto Prazo (Imediato)](#111-curto-prazo-imediato)
    - 11.2 [Médio Prazo (1-3 meses)](#112-médio-prazo-1-3-meses)
    - 11.3 [Longo Prazo (Trimestral/Anual)](#113-longo-prazo-trimestralanual)
12. [Conclusão](#12-conclusão)

---

## 1. Introdução

O Sistema de Gestão Portuária (SGP) é uma plataforma destinada a apoiar e otimizar todas as ações e operações efetuadas no porto. O principal objetivo é centralizar, organizar e tornar rastreável e controlável todas as informações relacionadas com navios (Vessels), recursos físicos (Physical Resources), pessoal logístico (Staff Member), áreas de armazenamento (Storage Area) e notificações de visita (Vessel Visit Notification), garantindo que as atividades decorrem de forma eficiente, segura e conforme os procedimentos da autoridade portuária.

### 1.1 Perfis do Sistema

O sistema é acedido por três perfis de utilizador principais:

- **Oficiais de Autoridade** (Port Authority Officer)
- **Operadores de Logística** (Logistics Operator)
- **Representantes de Agentes de Navegação** (Shipping Agent Representatives)

Cada um destes perfis desempenha as suas funções através de uma interface segura e unificada, com permissões adequadas ao seu papel.

### 1.2 Funcionalidades do Sistema

O SGP disponibiliza as seguintes funcionalidades:

- Gestão de Registo de Navios
- Gestão de Membros de Staff
- Gestão de Recursos Físicos
- Gestão de Áreas de Armazenamento
- Notificações da Visita de Navio
- Controlo de Acesso por papéis

---

## 2. Dados Pessoais Processados

O Sistema de Gestão Portuária processa vários tipos de dados pessoais, necessários para garantir o funcionamento seguro, rastreável e regulamentado das operações portuárias.

### 2.1 Tabela Resumida de Dados Pessoais

| Categoria de Titular | Dados Processados | Finalidade Principal | Base Legal (RGPD) |
|----------------------|-------------------|----------------------|-------------------|
| **Staff Members** | Nome, Número mecanográfico, Telefone curto, Email | Identificação interna, Comunicação profissional, Cumprimento de obrigações legais | Art. 6º (b), (c), (f) |
| **Shipping Agent Representatives** | Nome, Nacionalidade, Email profissional, Telefone | Autorização de submissão de VVN, Rastreabilidade de ações, Auditoria | Art. 6º (b), (e), (f) |
| **Crew Members** | Nome, Citizen ID, Nacionalidade | Segurança portuária, Verificação documental, Compliance marítima | Art. 6º (c), (e) |

---

## 3. Processamento de Dados Pessoais

O processamento ocorre nas fases de recolha, armazenamento, consulta, atualização, transmissão e retenção.

### 3.1 Dados de Staff Members

#### 3.1.1 Recolha

Introdução manual através da interface de criação/edição de funcionários feita por um responsável autorizado (Port Authority Officer).

#### 3.1.2 Armazenamento

Os dados são persistidos na base de dados operacional do sistema. Apenas atributos estritamente necessários são guardados.

#### 3.1.3 Utilização

Os dados de Staff Members são utilizados para:

- Identificação interna do staff
- Listagens e pesquisa de funcionários
- Atribuição de recursos, tarefas ou planeamento operacional
- Contacto, se necessário, por razões de segurança ou operação portuária

#### 3.1.4 Atualização

Apenas elementos autorizados podem atualizar dados de funcionários.

#### 3.1.5 Transmissão

Apenas transmitidos para o frontend através da API REST, sempre em ligações seguras (HTTPS). Não existe transferência destes dados para entidades externas.

#### 3.1.6 Retenção

Os dados permanecem enquanto o trabalhador estiver ativo e, após desativação, são mantidos apenas para fins de auditoria por um período máximo conforme legislação aplicável.

### 3.2 Dados de Shipping Agent Representatives

#### 3.2.1 Recolha

Introdução via SPA aquando da criação ou atualização de um representante associado a uma organização de agente de navegação.

#### 3.2.2 Armazenamento

Mantidos na base de dados como parte do agregado da Shipping Agent Representative.

#### 3.2.3 Utilização

Os dados são utilizados para:

- Autorização formal na submissão de Vessel Visit Notifications
- Identificação do representante responsável
- Comunicação operacional sobre notificações e documentação
- Auditoria de ações registadas no sistema

#### 3.2.4 Atualização

Apenas feita por administradores autorizados ou responsáveis designados.

#### 3.2.5 Transmissão

A informação é enviada ao frontend através de endpoints seguros (HTTPS). Não é partilhada com terceiros.

#### 3.2.6 Retenção

Mantida enquanto a organização estiver ativa ou existirem notificações associadas. Após término, conservar conforme período legal aplicável.

### 3.3 Dados de Crew Members

#### 3.3.1 Recolha

Os dados são introduzidos pelo Shipping Agent Representative no processo de criação de uma Vessel Visit Notification. Apenas recolhidos quando obrigatórios por regulamentos de segurança portuária.

#### 3.3.2 Armazenamento

Os dados são guardados dentro do agregado Vessel Visit Notification, como parte da estrutura do Crew Info.

#### 3.3.3 Utilização

Os dados são utilizados para:

- Verificação documental
- Identificação da tripulação presente num navio
- Cumprimento de regulamentos de segurança marítima

#### 3.3.4 Atualização

Apenas o representante que criou a notificação pode alterar enquanto o estado estiver "In Progress".

#### 3.3.5 Transmissão

Disponibilizados apenas às autoridades portuárias. Nunca enviados a entidades externas ou plataformas externas.

#### 3.3.6 Retenção

Mantidos enquanto a VVN estiver ativa e depois apenas para fins de auditoria, conforme regulamentação marítima internacional.

---

## 4. Fundamentos de Licitude (Artigo 6º do RGPD)

O RGPD estabelece seis possíveis bases legais que permitem o tratamento de dados pessoais. Dependendo da finalidade e do contexto, o tratamento pode apoiar-se em uma ou mais das seguintes bases:

- **Consentimento (alínea a)**: O titular dos dados dá o seu consentimento livre, específico, informado e inequívoco para uma finalidade determinada.

- **Execução de um contrato (alínea b)**: O tratamento é necessário para a execução de um contrato ou para diligências pré-contratuais a pedido do titular.

- **Cumprimento de obrigação legal (alínea c)**: O tratamento é exigido por lei, regulamento ou norma aplicável ao responsável pelo tratamento.

- **Proteção de interesses vitais (alínea d)**: O tratamento é necessário para proteger a vida ou integridade física do titular ou de outra pessoa.

- **Interesse público / exercício de autoridade pública (alínea e)**: O tratamento é necessário para o desempenho de funções de interesse público atribuídas por lei à entidade responsável.

- **Interesses legítimos (alínea f)**: O tratamento é necessário para fins de interesses legítimos do responsável ou de terceiros, desde que tais interesses não prevaleçam sobre os direitos e liberdades do titular.

### 4.1 Bases Legais por Categoria de Dados

#### 4.1.1 Dados de Staff Members

O tratamento destes dados baseia-se nas seguintes disposições legais:

- **Art. 6º, n.º 1, alínea (b)**: O tratamento é necessário para a execução do contrato laboral, justificando a identificação interna e a comunicação entre o trabalhador e a organização.

- **Art. 6º, n.º 1, alínea (c)**: O tratamento é necessário para o cumprimento de obrigações legais relacionadas com segurança no trabalho, auditoria, organização interna ou registos obrigatórios.

- **Art. 6º, n.º 1, alínea (f)**: Existe interesse legítimo da autoridade portuária em manter registos, auditorias internas e mecanismos de controlo de acessos que assegurem a segurança operacional e a rastreabilidade das ações efetuadas pelos funcionários.

#### 4.1.2 Dados de Shipping Agent Representatives

- **Art. 6º, n.º 1, alínea (f)**: O interesse legítimo da autoridade portuária justifica a necessidade de confirmar que o utilizador que interage com o sistema é um representante autorizado.

- **Art. 6º, n.º 1, alínea (b)**: O tratamento é necessário para o desempenho de atividades contratuais entre a autoridade portuária e a organização que o representante representa.

- **Art. 6º, n.º 1, alínea (e)**: O tratamento é necessário para a execução de tarefas de interesse público e exercício de autoridade pública delegada à autoridade portuária.

#### 4.1.3 Dados de Crew Members

- **Art. 6º, n.º 1, alínea (c)**: O tratamento é obrigatório para cumprir requisitos legais relacionados com a segurança marítima e protocolos internacionais.

- **Art. 6º, n.º 1, alínea (e)**: O tratamento é necessário para o desempenho de funções de interesse público e autoridade pública da administração portuária.

---

## 5. Princípios Gerais do RGPD (Artigo 5º)

Independentemente da categoria de titular, o sistema respeita os seguintes princípios fundamentais:

**Minimização, Limitação da Finalidade e Exatidão**: Apenas são recolhidos dados estritamente necessários, usados exclusivamente para finalidades legítimas e mantidos exatos e atualizados.

**Limitação da Conservação**: Os dados são conservados apenas durante o período necessário ao cumprimento das finalidades ou obrigações legais.

**Integridade e Confidencialidade**: Os dados são protegidos contra acessos não autorizados, danos ou perdas, com medidas técnicas e organizativas adequadas.

**Transparência**: Todos os titulares são informados sobre como os seus dados são recolhidos, tratados, armazenados e quais os direitos que lhes assistem ao abrigo do RGPD.

---

## 6. Medidas de Segurança (Artigo 32º do RGPD)

O sistema implementa as seguintes medidas de segurança técnicas e organizativas:

- **Encriptação**: Dados em trânsito (HTTPS/TLS) e em repouso no servidor.

- **Controlo de Acessos**: Autenticação multifator (onde aplicável) e controlo de permissões baseado em papéis (RBAC).

- **Auditoria e Monitorização**: Registos de todas as operações sobre dados pessoais para fins de compliance e investigação de incidentes.

- **Formação do Staff**: Programas de awareness em proteção de dados e segurança informática.

- **Plano de Contingência**: Procedimentos documentados para resposta a incidentes e recuperação de dados.

---

## 7. Procedimentos em Caso de Violação de Dados Pessoais

### 7.1 O que constitui uma Violação de Dados Pessoais

Uma violação de dados pessoais é qualquer incidente de segurança que resulte, de forma acidental ou ilícita, em:

- Acesso não autorizado a dados pessoais
- Alteração ou destruição de dados pessoais
- Perda de dados, permanente ou temporária
- Divulgação indevida dos dados a terceiros não autorizados
- Indisponibilidade dos dados que impeça o exercício de direitos ou comprometa a operação

#### 7.1.1 Exemplos de Violações

- Roubo de credenciais ou tokens de acesso
- Exposição pública de dados através de erro de configuração
- Envio de dados para o destinatário errado
- Falha de segurança no servidor
- Ataque informático (ransomware, SQL injection, etc.)
- Perda de equipamento contendo dados pessoais
- Acesso indevido por parte de utilizador interno

### 7.2 Procedimento de Notificação

Perante uma violação de dados pessoais, deve ser realizada uma análise imediata para determinar a gravidade e o risco para os titulares. Se o incidente for suscetível de resultar num risco para os direitos e liberdades das pessoas, aplicam-se as seguintes obrigações:

#### 7.2.1 Notificação à Autoridade de Controlo

Deve ser efetuada uma notificação à autoridade supervisora competente (em Portugal: Comissão Nacional de Proteção de Dados - CNPD), contendo obrigatoriamente:

- Natureza da violação
- Categorias e número aproximado de titulares afetados
- Categorias e quantidade de dados pessoais envolvidos
- Prováveis efeitos da violação
- Medidas adotadas ou propostas para mitigar danos
- Contacto do Data Protection Officer (DPO) ou ponto de contacto responsável

**Base Legal**: Art. 33º, n.º 3 do RGPD

#### 7.2.2 Notificação aos Titulares dos Dados

Se a violação for suscetível de implicar um elevado risco para os direitos e liberdades dos titulares dos dados, estes também devem ser notificados de forma clara e transparente. A comunicação deve incluir:

- Descrição da natureza da violação
- Contacto do responsável pela proteção de dados (DPO)
- Prováveis consequências da violação
- Medidas tomadas para mitigar danos
- Recomendações aos titulares para se protegerem (ex.: alteração de passwords)

**Base Legal**: Art. 34º do RGPD

### 7.3 Prazos Obrigatórios

A notificação deve ser realizada **sem demora injustificada** e, sempre que possível, **no prazo máximo de 72 horas** após ter tido conhecimento da violação.

Se a notificação ocorrer após o prazo, é obrigatório justificar o atraso.

**Base Legal**: Art. 33º, n.º 1 do RGPD

### 7.4 Fluxo de Ação em Caso de Incidente

1. **Deteção**: Identificação e confirmação da violação
2. **Isolamento**: Conter o incidente e evitar propagação
3. **Análise**: Avaliar âmbito, gravidade e risco para titulares
4. **Documentação**: Registar todos os detalhes do incidente
5. **Notificação CNPD**: Se necessário, no prazo de 72 horas
6. **Notificação Titulares**: Se risco elevado, comunicar diretamente
7. **Investigação**: Determinar causa raiz e medidas preventivas
8. **Seguimento**: Implementar melhorias de segurança

---

## 8. Direitos dos Titulares de Dados

O SGP respeita todos os direitos dos titulares de dados conforme previstos nos Artigos 12º a 23º do RGPD.

| Direito | Descrição | Artigo RGPD |
|---------|-----------|-------------|
| **Direito de Acesso** | Acesso aos dados pessoais em tratamento | Art. 15º |
| **Direito de Retificação** | Corrigir dados incorretos | Art. 16º |
| **Direito ao Apagamento** | Solicitar eliminação de dados | Art. 17º |
| **Direito à Limitação** | Restringir o tratamento | Art. 18º |
| **Direito de Portabilidade** | Receber dados num formato estruturado | Art. 20º |
| **Direito de Oposição** | Opor-se ao tratamento | Art. 21º |
| **Direito à Não Automatização** | Rejeitar decisões apenas automatizadas | Art. 22º |

Os titulares podem exercer estes direitos contactando o DPO ou a autoridade portuária através dos canais designados.

---

## 9. Avaliação de Impacto (DPIA)

Recomenda-se realizar uma **Avaliação de Impacto na Proteção de Dados (DPIA - Data Protection Impact Assessment)** conforme Art. 35º do RGPD, particularmente para:

- Implementação de novas funcionalidades de processamento automático
- Alterações significativas na estrutura de armazenamento de dados
- Integração com sistemas externos
- Mudanças nas políticas de retenção de dados

Uma DPIA documenta:

- Finalidades e benefícios esperados
- Análise de riscos para direitos e liberdades
- Medidas técnicas e organizativas implementadas
- Conformidade com regulamentações aplicáveis

---

## 10. Conformidade e Responsabilidades

### 10.1 Responsabilidades da Organização

A organização é responsável por:

- Manter documentação atualizada sobre tratamento de dados
- Implementar e testar medidas de segurança regularmente
- Realizar auditorias internas e externas de compliance
- Manter registos de todas as violações e incidentes
- Executar formação regular em GDPR e proteção de dados
- Designar um Data Protection Officer (DPO) responsável

### 10.2 Responsabilidades do Project Manager

O Project Manager é responsável por:

- Garantir que a equipa compreende as obrigações de GDPR
- Validar que todas as funcionalidades respeitam princípios de proteção de dados
- Comunicar violações às partes responsáveis imediatamente
- Coordenar com DPO em situações de risco
- Documentar decisões relacionadas com proteção de dados
- Facilitar formação contínua da equipa

---

## 11. Recomendações Práticas

### 11.1 Curto Prazo (Imediato)

1. **Formação Obrigatória**: Implementar sessão de awareness em GDPR para toda a equipa de desenvolvimento, especialmente sobre identificação de violações.

2. **Documentação de Retenção**: Especificar prazos exatos de retenção para cada categoria de dados (Staff, Shipping Agents, Crew Members) de acordo com legislação aplicável.

3. **Plano de Resposta a Incidentes**: Criar procedimento documentado com contactos, escalação e templates de comunicação.

### 11.2 Médio Prazo (1-3 meses)

4. **Designação de DPO**: Se não existir, designar Data Protection Officer responsável pelas operações de compliance.

5. **Testes de Segurança**: Realizar penetration testing e security audits ao sistema SGP.

6. **Auditoria de Acessos**: Rever permissões de utilizadores e implementar least privilege access.

7. **Política de Consentimento**: Documentar e validar consentimentos, especialmente para dados de terceiros.

### 11.3 Longo Prazo (Trimestral/Anual)

8. **DPIA Periódica**: Realizar avaliações de impacto quando ocorrem alterações significativas.

9. **Auditorias de Conformidade**: Realizar auditorias internas semestrais e externas anuais.

10. **Atualização de Políticas**: Rever e atualizar políticas de proteção de dados conforme mudanças legislativas ou operacionais.

11. **Monitorização de Regulamentações**: Acompanhar evoluções em lei marítima internacional que possam afetar tratamento de dados.

---

## 12. Conclusão

O Sistema de Gestão Portuária implementa um tratamento de dados pessoais em conformidade com o RGPD, respeitando os princípios de minimização, legítima base legal, segurança e transparência. A compreensão profunda destas obrigações por parte de toda a equipa, especialmente Project Manager, developers e DPO, é fundamental para manter a conformidade contínua e responder eficazmente a incidentes.

A adoção das recomendações propostas reforçará ainda mais a postura de proteção de dados da organização, demonstrando compromisso com a segurança dos titulares e conformidade regulatória.
