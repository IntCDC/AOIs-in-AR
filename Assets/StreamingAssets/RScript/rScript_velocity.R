setwd("C:/Users/seyda/Documents/Uni/Master/MA/master_thesis/VideoProcessing/Assets/RScript")
library("arett")

args <- commandArgs(trailingOnly = TRUE)

for(i in 1:length(args)){

  #path <- "C:/Users/seyda/Documents/Uni/Master/MA/master_thesis/VideoProcessing/Assets\\VideoProcessing/study/participant1/participant1.csv"
  path<- args[i]

  filename <-paste(tools::file_path_sans_ext(basename(path)),"_fixation.csv", sep="")
  newFilePath<-paste(dirname(path),"/",filename, sep="")
  print(newFilePath)

  data <- read.csv(file = path, header = TRUE)
  data$velocity <- NA
  data$gazeHasValue<-as.logical(data$gazeHasValue)
  data_modified<-calculate_velocity(data, window_length = 20)
  data_modified<-gap_fill(data_modified, max_gap_length = 105)
  data_modified_class_velocity<-classify_ivt(data_modified, velocity_threshold = 100)
  write.csv(data_modified_class_velocity,newFilePath, row.names = FALSE)

}

#
# path <- "C:/Users/seyda/Documents/Uni/Master/MA/master_thesis/VideoProcessing/Assets\\VideoProcessing/study/participant1/participant1.csv"
# #  path<- args[i]
#
#   filename <-paste(tools::file_path_sans_ext(basename(path)),"_fixation.csv", sep="")
#   newFilePath<-paste(dirname(path),"/",filename, sep="")
#   print(newFilePath)
#
#   data <- read.csv(file = path, header = TRUE)
#   data$velocity <- NA
#   data$gazeHasValue<-as.logical(data$gazeHasValue)
#   data_modified<-calculate_velocity(data, window_length = 20)
#   #data_modified<-gap_fill(data_modified, max_gap_length = 105)
#
#   data_modified_class_velocity<-classify_ivt(data_modified, velocity_threshold = 100)
#
#
#   #data_modified_class_velocity2<-merge_fixations_ivt(data_modified_class_velocity, max_time = 100, max_angle = 0.5)
#
#   #data_modified_class<-classify_idt(data_modified, dispersion_threshold = 1.6, time_window = 250)
#   #data_modified_class_merged<-merge_fixations_idt(data_modified_class, max_time = 105, dispersion_threshold = 1.6)
#   write.csv(data_modified_class,newFilePath, row.names = FALSE)
#
