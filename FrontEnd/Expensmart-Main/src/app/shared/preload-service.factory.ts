import { ValidationsServiceService } from "./validations-service.service";
export function PreloadFactory(configService: ValidationsServiceService) {
  return () => configService.initialize();
}