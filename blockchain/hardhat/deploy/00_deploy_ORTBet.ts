import { DeployFunction } from "hardhat-deploy/dist/types"
import { HardhatRuntimeEnvironment } from "hardhat/types"
import fs, { writeFile } from "fs"

const deployORTBet: DeployFunction = async (hre: HardhatRuntimeEnvironment) => {
  const { deployer } = await hre.getNamedAccounts()
  const { deploy } = hre.deployments

  console.log("deployer: ", deployer)

  const deployResult = await deploy("ORTBet", {
    from: deployer,
    args: [],
    log: true,
  })

  console.log("Contract ORTBet.sol deployed.")

  // Needed for Nethereum code generation.
  writeFile(
    "./artifacts/contracts/ORTBet.sol/ORTBet.abi",
    JSON.stringify(deployResult.abi),
    (err) => {
      if (err) console.error(err)
      else console.log("Abi file saved")
    }
  )

  const ortBet = await hre.ethers.getContractAt("ORTBet", deployResult.address)

  // console.log("transferring ownership...")
  // const ownerTx = await ortBet.transferOwnership("")
  // console.log("confirming...")
  // ownerTx.
}

export default deployORTBet

deployORTBet.tags = ["ORTBet"]
