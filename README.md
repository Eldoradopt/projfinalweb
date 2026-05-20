# Marketplace de Artigos em Segunda Mão #


###  Design & Experiência de Utilizador (UX/UI Premium)
*   **Estética Moderna:** Interface ultra-premium baseada na tipografia *Plus Jakarta Sans*, com sombras suaves, gradientes fluídos de cor Índigo/Slate e cantos arredondados.
*   **Menu Translucido (Glassmorphism):** Barra de navegação fixa com efeito desvanecido, pesquisa rápida integrada e rodapé institucional com links de contacto e ícones sociais da *Bootstrap Icons*.
*   **Homepage Reativa:** Banner principal apelativo, catálogo circular de categorias com carregamento em tempo real, e grelha dinâmica com os artigos mais recentes da plataforma.

###  Publicação de Anúncios Reativa (`/Anuncios/Create`)
*   **Pré-visualização Instantânea:** Campo inteligente em Javascript que carrega a imagem do produto em tempo real (URL ou upload de ficheiro local) no momento em que é preenchido.
*   **Modo de Demonstração (Demo Bypassing):** Caso o utilizador não tenha sessão iniciada, a plataforma associa automaticamente a publicação ao utilizador de demonstração (`demo@ipt.pt`), permitindo testar o fluxo de publicação imediato sem fricção de registo!

###  Catálogo Inteligente com Filtros e Ordenação (`/Anuncios/Index`)
*   **Filtros Avançados:** Barra lateral reativa com pesquisa de texto (título/descrição), filtro por categoria hierárquica (subcategorias) e filtros de preço mínimo e máximo.
*   **Ordenação Dinâmica:** Permite organizar instantaneamente o catálogo por "Mais Recentes", "Preço: Mais Baixo" e "Preço: Mais Alto".

###  Detalhes de Artigo Premium (`/Anuncios/Details`)
*   **Ficha Técnica do Produto:** Destaque de preço, estado físico de conservação (Novo, Como Novo, Usado, etc.) e data de publicação.
*   **Perfil do Vendedor Incorporado:** Cartão com a foto de perfil do vendedor, cidade de entrega em mãos, telefone para contacto telefónico direto e botão para início de chat privado.

---

##  Estrutura da Base de Dados (Entity Framework Core)

O projeto baseia-se numa arquitetura **Code-First** com as seguintes entidades relacionais estruturadas no SQL Server:

1.  **`ApplicationUser`**: Extensão do IdentityUser com atributos personalizados (Nome, Cidade, Morada, FotoPerfilPath, DataNascimento, DataRegisto).
2.  **`Categoria`**: Modelo auto-referencial que organiza de forma hierárquica categorias principais e subcategorias.
3.  **`Anuncio`**: Publicação de venda (Título, Descrição, Preço, Estado transacional e Condição física).
4.  **`Imagem`**: Ficheiros de imagem ou URLs associados aos produtos.
5.  (sob avalicao se deviam ser adicionados e a sua viabilidade)
6.  **`Mensagem`**: Registo de mensagens enviadas entre comprador e vendedor sobre um artigo. *********
7.  **`Avaliacao`**: Sistema de classificação pós-venda (1 a 5 estrelas).*********
8.  **`Favorito`**: Tabela N-M de ligação para favoritos utilizando chaves compostas. *********

> [!NOTE]
> Configurada a integridade referencial com `DeleteBehavior.Restrict` em relações cíclicas para evitar conflitos de cascata múltipla (`Error 1785`) no motor do SQL Server.

---

## 🛠️ Como Correr o Projeto Localmente

### Pré-requisitos
*   [.NET 10.0 SDK](https://dotnet.microsoft.com/download) instalado.
*   SQL Server LocalDB (incluído por defeito no Visual Studio).

### Passos de Execução
1.  **Clonar o Repositório:**
    ```bash
    git clone <url-do-teu-repositorio>
    cd projfinalweb/Proj_finalweb_loja/Proj_finalweb_loja
    ```

2.  **Instalar Dependências e Compilar:**
    ```bash
    dotnet restore
    dotnet build
    ```

3.  **Inicializar a Base de Dados (Migration & Update):**
    ```bash
    dotnet ef database update
    ```
    *Nota: A aplicação aplica o `DbInitializer` automaticamente ao iniciar, semeando as 5 categorias, 14 subcategorias, 3 contas de utilizador de teste e 5 anúncios premium de exemplo.*

4.  **Executar a Aplicação:**
    ```bash
    dotnet run
    ```
    Acede ao teu navegador em `http://localhost:5017` ou `https://localhost:7218`.

---

##  Contas de Demonstração (Semeadas)

Para facilidade de testes, a plataforma vem pré-carregada com as seguintes contas (palavra-passe comum: `Password123!`):

*   **Vendedor Demo:** `demo@ipt.pt` (Tomar)
*   **Maria Silva:** `maria@ipt.pt` (Abrantes)
*   **João Pereira:** `joao@ipt.pt` (Tomar)
