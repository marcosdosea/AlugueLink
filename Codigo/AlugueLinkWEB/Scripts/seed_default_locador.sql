-- Script para inserir locador padr�o se n�o existir
INSERT IGNORE INTO `locador` (`nome`, `email`, `telefone`, `cpf`) 
VALUES ('Locador Padr�o', 'locador@aluguelink.com', '11999999999', '00000000000');