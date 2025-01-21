import { DeployFunction } from "hardhat-deploy/dist/types"
import { HardhatRuntimeEnvironment } from "hardhat/types"

const deployORTBet: DeployFunction = async (hre: HardhatRuntimeEnvironment) => {
  const { deployer } = await hre.getNamedAccounts()
  const { deploy } = hre.deployments

  const deployResult = await deploy("ORTBet.sol", {
    from: deployer,
    args: [],
    log: true,
  })

  const ortBet = await hre.ethers.getContractAt("ORTBet", deployResult.address)

  console.log("Contract ORTBet.sol deployed.")

  // console.log("transferring ownership...")
  // const ownerTx = await ortBet.transferOwnership("")
  // console.log("confirming...")
  // ownerTx.
}
