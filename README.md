# Backend - Sistema de Gestão de Representação Comercial

Este documento especifica a arquitetura de dados, os requisitos de negócio e os critérios de aceitação 
técnicos para o motor de persistência e regras de negócio (Backend) do sistema. O projeto adota a arquitetura 
**Monolith First**, isolando completamente as regras de domínio da interface do usuário (MVVM/Avalonia UI).

O banco de dados utilizado é o **SQLite** em ambiente desktop, estruturado de forma estritamente relacional 
(3NF) para garantir a consistência, eliminar a duplicidade de dados e otimizar a performance local.

---

## 1. Dicionário de Dados e Modelo Relacional

As entidades abaixo estão mapeadas utilizando o padrão de Chaves Primárias (`PK`) 
baseadas em `Guid` e Chaves Estrangeiras (`FK`) para integridade referencial.

### Saller (Usuário/Vendedor)
* `Id`: Guid (PK)
* `Login`: String
* `Password`: String (Hash criptografado)
* `Name`: String

### RepresentedCompany (Representada/Fornecedor)
* `Id`: Guid (PK)
* `CompanyName`: String
* `CommissionPercentage`: Decimal

### Customer (Cliente)
* `Id`: Guid (PK)
* `Name`: String
* `Address`: String

### Product (Produto)
* `Id`: Guid (PK)
* `RepresentedCompanyId`: Guid (FK)
* `Description`: String
* `UnitPrice`: Decimal

### Sale (Venda/Pedido)
* `Id`: Guid (PK)
* `CustomerId`: Guid (FK)
* `RepresentedCompanyId`: Guid (FK)
* `SallerId`: Guid (FK)
* `SaleDate`: DateTime
* `TypeSale`: Enum (`Sale`, `FreeSample`)
* `Amount`: Decimal
* `Commission`: Decimal

### SaleItem (Itens da Venda)
* `Id`: Guid (PK)
* `SaleId`: Guid (FK)
* `ProductId`: Guid (FK)
* `Quantity`: Integer
* `PriceApplied`: Decimal

### Invoice (Faturamento)
* `Id`: Guid (PK)
* `SaleId`: Guid (FK) (Relacionamento 1:1 com a Venda)
* `InvoiceNumber`: Integer
* `IssueDate`: DateTime

### Installment (Parcelas)
* `Id`: Guid (PK)
* `InvoiceId`: Guid (FK)
* `DueDate`: DateTime
* `InstallmentValue`: Decimal
* `IsPaid`: Boolean

---

## 2. Domain-Driven Design (DDD)
```
├── 📁 src/
│   │
│   ├── 📁 1.Domain/                (O Coração do Sistema - Sem dependências externas)
│   │   ├── 📁 Entities/            (User, Sale, Customer, Installment...)
│   │   ├── 📁 Enums/               (TypeSale...)
│   │   ├── 📁 IRepositories/        (Interfaces de acesso a dados: ISaleRepository...)
│   │
│   ├── 📁 2.Application/           (A Ponte entre a UI e o Domínio)
│   │   ├── 📁 Services/            (Implementação dos fluxos de caso de uso)
│   │   └── 📁 Abstractions/        (Abstrações)
│   │
│   ├── 📁 3.Infrastructure/        (Acesso ao Mundo Externo)
│   │   ├── 📁 Data/                (Contexto do SQLite, Migrations, Repositories implementados)
│   │   └── 📁 Security/            (Criptografia de senha do User, Logs)
│   │
│   └── 📁 4.Presentation/          (A Camada Avalonia UI)
│       ├── 📁 Assets/              (Estilos, Imagens, Ícones)
│       ├── 📁 Views/               (XAML das Telas: SaleView.axaml, CustomerView.axaml)
│       └── 📁 ViewModels/          (Lógica da UI: SaleViewModel.cs, MainViewModel.cs)
```

## 3. Requisitos de Negócio (Business Rules)

As regras abaixo descrevem o comportamento esperado e as restrições que a 
camada de domínio(Business Logic) deve validar antes de persistir qualquer dado no SQLite.

### Escopo e Vínculos da Venda (`Sale`)
* **Exclusividade de Representada:** Uma `Sale` deve conter apenas produtos (`Product`) 
que pertençam à `RepresentedCompanyId` informada no cabeçalho do pedido. O sistema deve rejeitar 
a inserção de um `SaleItem` cujo produto pertença a outra representada.
* **Vendedor Obrigatório:** Toda `Sale` precisa estar associada ao `UserId` do usuário que a emitiu, 
servindo de base para relatórios de desempenho e auditoria.

### Cálculos e Totais de Venda e Itens
* **Cálculo do `Amount` da Venda:** O campo `Amount` da entidade `Sale` não é inserido manualmente. 
Ele deve ser o resultado exato do somatório de (`Quantity` $\times$ `PriceApplied`) de todos os 
registros de `SaleItem` vinculados àquela `Sale`.
* **Cálculo da `Commission` (Comissão):** * Se o `TypeSale` for igual a `Sale`, 
o campo `Commission` deve ser calculado multiplicando o `Amount` total do pedido pelo percentual 
de comissão (`CommissionPercentage`) cadastrado na `RepresentedCompany` correspondente.
* Se o `TypeSale` for igual a `FreeSample` (Amostra Grátis), os campos `Amount` e `Commission` 
devem ser obrigatoriamente zerados ($0.0$), independentemente dos itens adicionados.

### Fluxo de Faturamento (`Invoice`) e Parcelamento (`Installment`)
* **Elegibilidade de Faturamento:** Uma `Invoice` só pode ser gerada para vendas concluídas 
cujo `TypeSale` seja igual a `Sale`. Vendas do tipo `FreeSample` estão bloqueadas para faturamento.
* **Consistência do Parcelamento:** O somatório de todos os valores de `InstallmentValue` 
de todas as parcelas (`Installment`) vinculadas a uma `Invoice` deve ser matematicamente igual ao valor
armazenado no campo `Commission` da respectiva `Sale`. O sistema deve impedir o salvamento se houver
divergência de centavos.
* **Controle de Liquidação:** A propriedade `IsPaid` controla o status de recebimento da comissão. 
Uma parcela só pode ser marcada como `IsPaid = true` mediante uma ação de baixa financeira no sistema, 
representando que a fábrica realizou o pagamento daquela fração da comissão.

---

## 4. Critérios de Aceitação Técnicos (Mapeamento e SQLite)

### Alerta de Arquitetura: Precisão Numérica Financeira
* Embora o modelo relacional acima liste propriedades como `Amount`, `Commission`, `PriceApplied` e
`InstallmentValue` como `Double` (tipagem comum em representações visuais), 
**o código C#/Dotnet do backend DEVE obrigatoriamente mapear esses campos como `decimal`**.
* O SQLite não possui um tipo `decimal` nativo (ele armazena como `REAL`/Ponto Flutuante), 
portanto, o mecanismo de persistência (EF Core, Dapper ou ADO.NET) deve ser configurado para 
armazenar esses campos como `TEXT` ou `NUMERIC` com precisão fixa, evitando erros de arredondamento 
cumulativos no fechamento do caixa do escritório.

### Integridade Referencial do SQLite
* **Ativação de Foreign Keys:** O mecanismo de persistência deve enviar o comando 
`PRAGMA foreign_keys = ON;` ao SQLite em toda abertura de conexão, garantindo a validação de chaves 
estrangeiras em tempo de execução.
* **Deleção em Cascata (Cascade Delete):** Ao excluir uma `Sale`, o banco de dados deve remover 
automaticamente todos os registros dependentes na tabela `SaleItem`. Se uma `Invoice` for excluída, 
todas as suas `Installment` devem cair em cascata.
* **Restrição de Exclusão (Restrict):** O sistema deve bloquear a exclusão de um `Product`, 
`Customer`, `User` ou `RepresentedCompany` se o ID do registro estiver associado a qualquer venda
ativa no histórico.

