-- ======================================================
-- SCRIPT DO BANCO DE DADOS: ALUGUELINK
-- ======================================================

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, 
    SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,
    NO_ZERO_IN_DATE,NO_ZERO_DATE,
    ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- ------------------------------------------------------
-- SCHEMA
-- ------------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `aluguelink` 
    DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci ;
USE `aluguelink`;

-- ------------------------------------------------------
-- TABELA: Locador
-- ------------------------------------------------------
CREATE TABLE IF NOT EXISTS `locador` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `nome` VARCHAR(100) NOT NULL,
  `email` VARCHAR(100) NOT NULL,
  `telefone` VARCHAR(20) NOT NULL,
  `cpf` CHAR(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB;

-- ------------------------------------------------------
-- TABELA: Imovel
-- ------------------------------------------------------
CREATE TABLE IF NOT EXISTS `imovel` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `cep` VARCHAR(8) NOT NULL,
  `logradouro` VARCHAR(100) NOT NULL,
  `numero` VARCHAR(5) NOT NULL,
  `complemento` VARCHAR(50) NULL,
  `bairro` VARCHAR(50) NOT NULL,
  `cidade` VARCHAR(45) NOT NULL,
  `estado` CHAR(2) NOT NULL,
  `tipo` ENUM('C', 'A', 'PC') NOT NULL,
  `quartos` INT NOT NULL,
  `banheiros` INT NOT NULL,
  `area` DECIMAL(10,2) NOT NULL,
  `vagasGaragem` INT NOT NULL,
  `valor` DECIMAL(10,2) NOT NULL,
  `descricao` VARCHAR(200) NOT NULL,
  `idLocador` INT NOT NULL,
  PRIMARY KEY (`id`, `idLocador`),
  INDEX `fk_imovel_locador_idx` (`idLocador` ASC) VISIBLE,
  CONSTRAINT `fk_imovel_locador`
    FOREIGN KEY (`idLocador`)
    REFERENCES `locador` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
) ENGINE=InnoDB;

-- ------------------------------------------------------
-- TABELA: Locatario
-- ------------------------------------------------------
CREATE TABLE IF NOT EXISTS `locatario` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `nome` VARCHAR(100) NOT NULL,
  `email` VARCHAR(100) NOT NULL,
  `telefone` VARCHAR(20) NOT NULL,
  `cpf` CHAR(11) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `cpf_UNIQUE` (`cpf` ASC) VISIBLE
) ENGINE=InnoDB;

-- ------------------------------------------------------
-- TABELA: Aluguel
-- ------------------------------------------------------
CREATE TABLE IF NOT EXISTS `aluguel` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `dataInicio` DATE NOT NULL,
  `dataFim` DATE NOT NULL,
  `status` ENUM('A', 'F', 'P') NULL,
  `dataAssinatura` DATE NOT NULL,
  `idLocatario` INT NOT NULL,
  `idImovel` INT NOT NULL,
  `idLocador` INT NOT NULL,
  PRIMARY KEY (`id`, `idLocatario`, `idImovel`, `idLocador`),
  INDEX `fk_aluguel_locatario1_idx` (`idLocatario` ASC) VISIBLE,
  INDEX `fk_aluguel_imovel1_idx` (`idImovel` ASC, `idLocador` ASC) VISIBLE,
  CONSTRAINT `fk_aluguel_locatario1`
    FOREIGN KEY (`idLocatario`)
    REFERENCES `locatario` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_aluguel_imovel1`
    FOREIGN KEY (`idImovel`, `idLocador`)
    REFERENCES `imovel` (`id`, `idLocador`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
) ENGINE=InnoDB;

-- ------------------------------------------------------
-- TABELA: Pagamento
-- ------------------------------------------------------
CREATE TABLE IF NOT EXISTS `pagamento` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `valor` DECIMAL(10,2) NOT NULL,
  `dataPagamento` DATE NOT NULL,
  `tipoPagamento` ENUM('CD', 'CC', 'P', 'B') NOT NULL,
  `idAluguel` INT NOT NULL,
  PRIMARY KEY (`id`, `idAluguel`),
  INDEX `fk_pagamento_aluguel1_idx` (`idAluguel` ASC) VISIBLE,
  CONSTRAINT `fk_pagamento_aluguel1`
    FOREIGN KEY (`idAluguel`)
    REFERENCES `aluguel` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
) ENGINE=InnoDB;

-- ------------------------------------------------------
-- TABELA: Manutencao
-- ------------------------------------------------------
CREATE TABLE IF NOT EXISTS `manutencao` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `descricao` VARCHAR(200) NOT NULL,
  `dataSolicitacao` DATE NOT NULL,
  `status` ENUM('P', 'A', 'C') NULL,
  `valor` DECIMAL(10,2) NOT NULL,
  `idImovel` INT NOT NULL,
  `idLocador` INT NOT NULL,
  PRIMARY KEY (`id`, `idImovel`, `idLocador`),
  INDEX `fk_manutencao_imovel1_idx` (`idImovel` ASC, `idLocador` ASC) VISIBLE,
  CONSTRAINT `fk_manutencao_imovel1`
    FOREIGN KEY (`idImovel`, `idLocador`)
    REFERENCES `imovel` (`id`, `idLocador`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
) ENGINE=InnoDB;

-- ------------------------------------------------------
-- TABELA: locatario_has_locador
-- ------------------------------------------------------
CREATE TABLE IF NOT EXISTS `locatario_has_locador` (
  `idLocatario` INT NOT NULL,
  `idLocador` INT NOT NULL,
  PRIMARY KEY (`idLocatario`, `idLocador`),
  INDEX `fk_locatario_has_locador_locador1_idx` (`idLocador` ASC) VISIBLE,
  INDEX `fk_locatario_has_locador_locatario1_idx` (`idLocatario` ASC) VISIBLE,
  CONSTRAINT `fk_locatario_has_locador_locatario1`
    FOREIGN KEY (`idLocatario`)
    REFERENCES `locatario` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_locatario_has_locador_locador1`
    FOREIGN KEY (`idLocador`)
    REFERENCES `locador` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
) ENGINE=InnoDB;

-- ======================================================
-- RESTAURA CONFIGS ORIGINAIS
-- ======================================================
SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
