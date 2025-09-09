-- Script para inserir locador padrão se não existir
INSERT IGNORE INTO `locador` (`nome`, `email`, `telefone`, `cpf`) 
VALUES ('Locador Padrão', 'locador@aluguelink.com', '11999999999', '00000000000');