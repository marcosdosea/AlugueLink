-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------
-- -----------------------------------------------------
-- Schema aluguelink
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema aluguelink
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `aluguelink` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci ;
USE `aluguelink` ;

-- -----------------------------------------------------
-- Table `aluguelink`.`locador`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aluguelink`.`locador` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `nome` VARCHAR(100) NOT NULL,
  `email` VARCHAR(100) NOT NULL,
  `telefone` VARCHAR(20) NOT NULL,
  `cpf` CHAR(11) NOT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
AUTO_INCREMENT = 2
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_unicode_ci;


-- -----------------------------------------------------
-- Table `aluguelink`.`imovel`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aluguelink`.`imovel` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `cep` VARCHAR(8) NOT NULL,
  `logradouro` VARCHAR(100) NOT NULL,
  `numero` VARCHAR(5) NOT NULL,
  `complemento` VARCHAR(50) NULL DEFAULT NULL,
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
    REFERENCES `aluguelink`.`locador` (`id`))
ENGINE = InnoDB
AUTO_INCREMENT = 2
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_unicode_ci;


-- -----------------------------------------------------
-- Table `aluguelink`.`locatario`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aluguelink`.`locatario` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `nome` VARCHAR(100) NOT NULL,
  `email` VARCHAR(100) NOT NULL,
  `telefone` VARCHAR(20) NOT NULL,
  `cpf` CHAR(11) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `cpf_UNIQUE` (`cpf` ASC) VISIBLE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_unicode_ci;


-- -----------------------------------------------------
-- Table `aluguelink`.`aluguel`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aluguelink`.`aluguel` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `dataInicio` DATE NOT NULL,
  `dataFim` DATE NOT NULL,
  `status` ENUM('A', 'F', 'P') NULL DEFAULT NULL,
  `dataAssinatura` DATE NOT NULL,
  `idLocatario` INT NOT NULL,
  `idImovel` INT NOT NULL,
  `idLocador` INT NOT NULL,
  PRIMARY KEY (`id`, `idLocatario`, `idImovel`, `idLocador`),
  INDEX `fk_aluguel_locatario1_idx` (`idLocatario` ASC) VISIBLE,
  INDEX `fk_aluguel_imovel1_idx` (`idImovel` ASC, `idLocador` ASC) VISIBLE,
  CONSTRAINT `fk_aluguel_imovel1`
    FOREIGN KEY (`idImovel` , `idLocador`)
    REFERENCES `aluguelink`.`imovel` (`id` , `idLocador`),
  CONSTRAINT `fk_aluguel_locatario1`
    FOREIGN KEY (`idLocatario`)
    REFERENCES `aluguelink`.`locatario` (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_unicode_ci;


-- -----------------------------------------------------
-- Table `aluguelink`.`locatariolocador`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aluguelink`.`locatariolocador` (
  `idlocatario` INT NOT NULL,
  `idlocador` INT NOT NULL,
  PRIMARY KEY (`idlocatario`, `idlocador`),
  INDEX `fk_locatariolocador_locador1_idx` (`idlocador` ASC) VISIBLE,
  INDEX `fk_locatariolocador_locatario1_idx` (`idlocatario` ASC) VISIBLE,
  CONSTRAINT `fk_locatariolocador_locador1`
    FOREIGN KEY (`idlocador`)
    REFERENCES `aluguelink`.`locador` (`id`)
    ON DELETE RESTRICT
    ON UPDATE RESTRICT,
  CONSTRAINT `fk_locatariolocador_locatario1`
    FOREIGN KEY (`idlocatario`)
    REFERENCES `aluguelink`.`locatario` (`id`)
    ON DELETE RESTRICT
    ON UPDATE RESTRICT)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_unicode_ci;


-- -----------------------------------------------------
-- Table `aluguelink`.`manutencao`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aluguelink`.`manutencao` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `descricao` VARCHAR(200) NOT NULL,
  `dataSolicitacao` DATE NOT NULL,
  `status` ENUM('P', 'A', 'C') NULL DEFAULT NULL,
  `valor` DECIMAL(10,2) NOT NULL,
  `idImovel` INT NOT NULL,
  `idLocador` INT NOT NULL,
  PRIMARY KEY (`id`, `idImovel`, `idLocador`),
  INDEX `fk_manutencao_imovel1_idx` (`idImovel` ASC, `idLocador` ASC) VISIBLE,
  CONSTRAINT `fk_manutencao_imovel1`
    FOREIGN KEY (`idImovel` , `idLocador`)
    REFERENCES `aluguelink`.`imovel` (`id` , `idLocador`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_unicode_ci;


-- -----------------------------------------------------
-- Table `aluguelink`.`pagamento`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aluguelink`.`pagamento` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `valor` DECIMAL(10,2) NOT NULL,
  `dataPagamento` DATE NOT NULL,
  `tipoPagamento` ENUM('CD', 'CC', 'P', 'B') NOT NULL,
  `idAluguel` INT NOT NULL,
  PRIMARY KEY (`id`, `idAluguel`),
  INDEX `fk_pagamento_aluguel1_idx` (`idAluguel` ASC) VISIBLE,
  CONSTRAINT `fk_pagamento_aluguel1`
    FOREIGN KEY (`idAluguel`)
    REFERENCES `aluguelink`.`aluguel` (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_unicode_ci;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
