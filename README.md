
# 🛒 Ecomm 

Este é um projeto de demonstração de um sistema de ecommerce baseado em microserviços. O objetivo é explorar práticas modernas de desenvolvimento backend, como uso de **RabbitMQ** para comunicação assíncrona, e design modular com **DDD (Domain-Driven Design)**, **CQRS (Command Query Responsibility Segregation)** e **Clean Architecture**.

## 🛠️ Tecnologias Utilizadas

- **.NET 9**  
- **ASP.NET Core**  
- **Entity Framework Core**  
- **RabbitMQ**  
- **PostgreSQL**  
- **Docker** e **Docker Compose**  
- **Scalar** para documentação de APIs  

---

## 🏰 Arquitetura do Projeto

Este projeto segue os princípios de **DDD**, **CQRS** e **Clean Architecture**, garantindo que as regras de negócio fiquem isoladas de detalhes de implementação, como frameworks e banco de dados.

### Camadas Principais

1. **Domain**
   - Contém entidades, value objects, domain events e interfaces dos repositórios.  
   - Representa as regras de negócio centrais.

2. **Application**
   - Contém os casos de uso (application services) que orquestram as regras de negócio.

3. **Infrastructure**
   - Implementação de repositórios, integrações externas e configurações de banco de dados.

4. **Api**
   - APIs RESTful para comunicação com os clientes.

### CQRS

A separação entre comandos e consultas foi implementada para melhorar a escalabilidade e a clareza do código. 
- **Comandos**: Responsáveis por alterar o estado do sistema.  
- **Consultas**: Responsáveis por buscar dados sem alterar o estado.

### Microserviços

O sistema é composto pelos seguintes microserviços:

1. **Catalog**  
   - Gerenciamento de produtos e categorias (CRUD).
   - Exposição de APIs REST.

2. **Orders**  
   - Criação e gerenciamento de pedidos.  
   - Publica mensagens no RabbitMQ para processar pedidos.
   - Consome mensagens do RabbitMQ para atualizar a entidade de produto local.

3. **Payments**  
   - Processamento de pagamentos.  
   - Consome mensagens do RabbitMQ e responde ao serviço de pedidos.

4. **Customers**  
   - Gerenciamento de informações de clientes.
   - Publica mensgens no RabbitMQ para informar que um novo cliente foi cadastrado.

---

## 🔧 Configuração e Execução

### Pré-requisitos

- **.NET 9**.
- **Docker** e **Docker Compose** instalados.

### Passos para executar

1. Clone este repositório:  
   ```bash
   git clone https://github.com/raffreitasx/ecomm.git
   cd ecomm
   ```

2. Configure as variáveis de ambiente para cada serviço. Ou utilize a padrão para desenvolvimento local disponível no arquivo `appsettings.Development.json.`.

3. Suba os containers com o Docker Compose:  
   ```bash
   docker-compose up -d
   ```

4. Acesse a documentação das APIs através do Scalar:  
   - Catalog: [http://localhost:8080/scalar/v1](http://localhost:8080/scalar/v1)  
   - Order: [http://localhost:8081/scalar/v1](http://localhost:8081/scalar/v1)  
   - Payments: [http://localhost:8082/scalar/v1](http://localhost:8082/scalar/v1)  
   - Customers: [http://localhost:8083/scalar/v1](http://localhost:8083/scalar/v1)  

---

## 🚀 Funcionalidades

- CRUD de produtos e categorias.  
- Fluxo de criação de pedidos com publicação e consumo de mensagens.  
- Processamento de pagamentos integrado.

---

## 📢 Contribuições

Sinta-se à vontade para abrir **issues** ou enviar **pull requests** para melhorar este projeto.
